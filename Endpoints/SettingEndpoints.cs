using System.Security.Claims;
using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;
using ShareSpaceApi.Repository.Contracts;

namespace ShareSpaceApi.Endpoints;

public class SettingEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("settings")
            .RequireAuthorization(options => options.RequireRole("user"));

        group.MapPut("profile", UpdateProfile);
        group.MapPut("password", UpdatePassword);
    }

    public static async Task<
        Results<Ok<ApiResponse<AuthResponse>>, BadRequest<ApiResponse<AuthResponse>>>
    > UpdateProfile(
        ClaimsPrincipal user,
        ISettingsRepository setting_rep,
        [FromBody] UpdateUserProfileDto profileDto
    )
    {
        try
        {
            Guid UserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await setting_rep.UpdateProfile(profileDto, UserId);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<AuthResponse>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }

    public static async Task<
        Results<Ok<ApiResponse<string>>, BadRequest<ApiResponse<string>>>
    > UpdatePassword(
        ClaimsPrincipal user,
        ISettingsRepository setting_rep,
        [FromBody] UpdatePasswordDto passwordDto
    )
    {
        try
        {
            Guid UserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await setting_rep.UpdatePassword(passwordDto, UserId);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }
}
