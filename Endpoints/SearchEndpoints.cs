using System.Security.Claims;
using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;
using ShareSpaceApi.Repository.Contracts;

namespace ShareSpaceApi.Endpoints;

public class SearchEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("search")
            .RequireAuthorization(options => options.RequireRole("user"));

        group.MapGet("user/{query}", SearchUser);
        group.MapGet("post/{query}", SearchPost);
    }

    public static async Task<
        Results<
            Ok<ApiResponse<IEnumerable<UserSearchDto>>>,
            BadRequest<ApiResponse<IEnumerable<UserSearchDto>>>
        >
    > SearchUser([FromRoute] string query, ClaimsPrincipal user, ISearchRepository search_rep)
    {
        try
        {
            Guid UserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await search_rep.SearchUser(query, UserId);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<IEnumerable<UserSearchDto>>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }

    public static async Task<
        Results<
            Ok<ApiResponse<IEnumerable<PostSearchDto>>>,
            BadRequest<ApiResponse<IEnumerable<PostSearchDto>>>
        >
    > SearchPost([FromRoute] string query, ClaimsPrincipal user, ISearchRepository search_rep)
    {
        try
        {
            Guid UserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await search_rep.SearchPost(query, UserId);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<IEnumerable<PostSearchDto>>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }
}
