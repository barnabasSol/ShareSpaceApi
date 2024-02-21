using System.Security.Claims;
using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;
using ShareSpaceApi.Repository.Contracts;

namespace ShareSpaceApi.Endpoints;

public class UserEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("user")
            .RequireAuthorization(options => options.RequireRole("user"));

        group.MapGet("interests", GetInterests);
        group.MapPost("store-interests", StoreInterests);
        group.MapGet("extra-info/{userid:guid}", GetExtraInfo);
        group.MapGet("profile/{username}", GetProfile);
        group.MapPut("follow/{userid:guid}", FollowUser);
        group.MapGet("suggested-users", GetSuggestedUsers);
        group.MapGet("followers", GetFollowers);
        group.MapGet("following", GetFollowing);
    }

    public static async Task<
        Results<
            Ok<ApiResponse<IEnumerable<InterestsDto>>>,
            BadRequest<ApiResponse<IEnumerable<InterestsDto>>>
        >
    > GetInterests(ClaimsPrincipal user, IUserRepository user_rep)
    {
        try
        {
            Guid UserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await user_rep.GetInterests();
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<IEnumerable<InterestsDto>>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }

    public static async Task<
        Results<Ok<ApiResponse<string>>, BadRequest<ApiResponse<string>>>
    > StoreInterests(
        ClaimsPrincipal user,
        [FromBody] IEnumerable<InterestsDto> interests,
        IUserRepository user_rep
    )
    {
        try
        {
            Guid UserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await user_rep.StoreInterests(interests, UserId);
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

    public static async Task<
        Results<Ok<ApiResponse<ExtraUserInfoDto>>, BadRequest<ApiResponse<ExtraUserInfoDto>>>
    > GetExtraInfo([FromRoute] Guid userid, IUserRepository user_rep)
    {
        try
        {
            var response = await user_rep.GetExtraUserInfo(userid);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<ExtraUserInfoDto>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }

    public static async Task<
        Results<Ok<ApiResponse<ProfileDto>>, BadRequest<ApiResponse<ProfileDto>>>
    > GetProfile([FromRoute] string username, ClaimsPrincipal user, IUserRepository user_rep)
    {
        try
        {
            Guid current_user_id = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await user_rep.GetProfile(username, current_user_id);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<ProfileDto>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }

    public static async Task<
        Results<Ok<ApiResponse<string>>, BadRequest<ApiResponse<string>>>
    > FollowUser([FromRoute] Guid userid, ClaimsPrincipal user, IUserRepository user_rep)
    {
        try
        {
            Guid current_user_id = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await user_rep.FollowUser(
                followed_id: userid,
                follower_id: current_user_id
            );
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

    public static async Task<
        Results<
            Ok<ApiResponse<IEnumerable<SuggestedUserDto>>>,
            BadRequest<ApiResponse<IEnumerable<SuggestedUserDto>>>
        >
    > GetSuggestedUsers(ClaimsPrincipal user, IUserRepository user_rep)
    {
        try
        {
            Guid current_user_id = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await user_rep.GetSuggestedUsers(current_user_id);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<IEnumerable<SuggestedUserDto>>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }

    public static async Task<
        Results<
            Ok<ApiResponse<IEnumerable<FollowerUserDto>>>,
            BadRequest<ApiResponse<IEnumerable<FollowerUserDto>>>
        >
    > GetFollowers(ClaimsPrincipal user, IUserRepository user_rep)
    {
        try
        {
            Guid current_user_id = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await user_rep.GetFollowers(current_user_id);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<IEnumerable<FollowerUserDto>>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }

    public static async Task<
        Results<
            Ok<ApiResponse<IEnumerable<FollowerUserDto>>>,
            BadRequest<ApiResponse<IEnumerable<FollowerUserDto>>>
        >
    > GetFollowing(ClaimsPrincipal user, IUserRepository user_rep)
    {
        try
        {
            Guid current_user_id = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await user_rep.GetFollowers(current_user_id);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<IEnumerable<FollowerUserDto>>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }
}
