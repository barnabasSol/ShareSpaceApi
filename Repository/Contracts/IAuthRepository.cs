using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;
using ShareSpaceApi.Data.Models;

namespace ShareSpaceApi.Repository.Contracts;

public interface IAuthRepository
{
    Task<AuthResponse> CreateUser(CreateUserDTO user);
    Task<AuthResponse> LoginUser(UserLoginDTO login);
    string GenerateAccessToken(User authorized_user, Role role);
    Task<string> GenerateRefershToken(Guid authorized_user_id);
}
