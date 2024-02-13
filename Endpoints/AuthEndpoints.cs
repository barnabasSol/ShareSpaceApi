using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;
using ShareSpaceApi.Repository.Contracts;

namespace ShareSpaceApi.Endpoints;

public class AuthEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("auth");
        group.MapPost("create-user", CreateUser);
        group.MapPost("login", LoginUser);
    }

    public static async Task<Results<Ok<AuthResponse>, BadRequest<AuthResponse>>> LoginUser(
        UserLoginDTO user,
        IAuthRepository auth
    )
    {
        try
        {
            var response = await auth.LoginUser(user);
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new AuthResponse
            {
                IsSuccess = false,
                Message = "Something went wrong, try again later." + ex.Message
            };
            return TypedResults.BadRequest(errorResponse);
        }
    }

    public static async Task<Results<Ok<AuthResponse>, BadRequest<AuthResponse>>> CreateUser(
        CreateUserDTO user,
        IAuthRepository auth
    )
    {
        try
        {
            var response = await auth.CreateUser(user);
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Something went wrong, try again later." + ex.Message
                }
            );
        }
    }
}
