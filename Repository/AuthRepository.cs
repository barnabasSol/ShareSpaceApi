﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShareSpaceApi.Data.Context;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;
using ShareSpaceApi.Data.Models;
using ShareSpaceApi.Extensions;
using ShareSpaceApi.Jwt;
using ShareSpaceApi.Repository.Contracts;
using BC = BCrypt.Net.BCrypt;

namespace ShareSpaceApi.Repository;

public enum Role
{
    User = 1,
    Admin = 2
}

public class AuthRepository(ShareSpaceDbContext shareSpaceDb, IOptions<TokenSettings> token_setting) : IAuthRepository
{
    private readonly ShareSpaceDbContext shareSpaceDb = shareSpaceDb;
    private readonly TokenSettings token_Setting = token_setting.Value;

    public async Task<AuthResponse> CreateUser(CreateUserDTO new_user)
    {
        using var transaction = await shareSpaceDb.Database.BeginTransactionAsync();
        try
        {
            if (await new_user.EmaiOrUsernameExists(shareSpaceDb.Users))
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "username or email is in use"
                };
            }

            User NewUser =
                new()
                {
                    UserName = new_user.UserName,
                    Name = new_user.Name,
                    Email = new_user.Email,
                    PasswordHash = BC.HashPassword(new_user.Password)
                };

            await shareSpaceDb.Users.AddAsync(NewUser);
            await shareSpaceDb.SaveChangesAsync();

            UserRole newUserRole = new() { UserId = NewUser.UserId, RoleId = (int)Role.User };
            await shareSpaceDb.UserRoles.AddAsync(newUserRole);
            await shareSpaceDb.SaveChangesAsync();

            await transaction.CommitAsync();

            return new AuthResponse()
            {
                IsSuccess = true,
                Message = "",
                AccessToken = GenerateAccessToken(NewUser, Role.User),
                RefreshToken = await GenerateRefershToken(NewUser.UserId)
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception(ex.Message);
        }
    }

    public async Task<AuthResponse> LoginUser(UserLoginDTO user_login)
    {
        try
        {
            if (
                string.IsNullOrEmpty(user_login.UserName)
                || string.IsNullOrEmpty(user_login.Password)
            )
            {
                return new AuthResponse { IsSuccess = false, Message = "enter the required field" };
            }

            var queried_user = await shareSpaceDb.Users
                .Where(_ => _.UserName == user_login.UserName)
                .FirstOrDefaultAsync();

            if (queried_user is null)
            {
                return new AuthResponse { IsSuccess = false, Message = "user doesn't exist" };
            }

            if (!BC.Verify(user_login.Password, queried_user.PasswordHash))
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "incorrect password or username"
                };
            }

            return new AuthResponse
            {
                IsSuccess = true,
                AccessToken = GenerateAccessToken(queried_user, Role.User),
                RefreshToken = await GenerateRefershToken(queried_user.UserId)
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public string GenerateAccessToken(User authorized_user, Role role)
    {
        var TokenExpiration = DateTime.Now.AddHours(15);
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(token_Setting.SecretKey));
        DateTimeOffset dateTimeOffset = new(TokenExpiration);
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        Claim[] claims =
            [
                new Claim(ClaimTypes.NameIdentifier, authorized_user.UserName),
                new Claim("Sub", authorized_user.UserId.ToString()),
                new Claim("UserName", authorized_user.UserName),
                new Claim("Name", authorized_user.Name),
                new Claim("Email", authorized_user.Email),
                new Claim(ClaimTypes.Role, role == Role.User ? "user" : "admin"),
                new Claim("exp", dateTimeOffset.ToUnixTimeSeconds().ToString()),
            ];
        JwtSecurityToken securityToken =
            new(
                issuer: token_Setting.Issuer,
                audience: token_Setting.Audience,
                expires: TokenExpiration,
                signingCredentials: credentials,
                claims: claims
            );
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public async Task<string> GenerateRefershToken(Guid authorized_user_id)
    {
        var TokenBytes = new byte[32];
        using var range = RandomNumberGenerator.Create();
        range.GetBytes(TokenBytes);
        var token = Convert.ToBase64String(TokenBytes);
        RefreshToken refreshToken =
            new()
            {
                ExpirationDate = DateTime.Now.AddMinutes(1),
                Token = token,
                UserId = authorized_user_id,
            };
        await shareSpaceDb.RefreshTokens.AddAsync(refreshToken);
        await shareSpaceDb.SaveChangesAsync();
        return token;
    }
}
