using System.Security.Claims;
using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;
using ShareSpaceApi.Repository.Contracts;

namespace ShareSpaceApi.Endpoints;

public class PostEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("post").RequireAuthorization(o => o.RequireRole("user"));
        group.MapGet("", GetPosts);
        group.MapGet("trending", GetTrendingPosts);
        group.MapPost("create", CreatePost);
        group.MapDelete("delete/{postid:guid}", DeletePost);
        group.MapGet("{postid:guid}", GetPost);
        group.MapPut("like", Like);
    }

    public static async Task<
        Results<Ok<ApiResponse<string>>, BadRequest<ApiResponse<string>>>
    > CreatePost(CreatePostDto post, IPostRepository post_rep, ClaimsPrincipal user)
    {
        try
        {
            post.PostedUserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await post_rep.CreatePost(post);
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

    public async Task<
        Results<
            Ok<ApiResponse<IEnumerable<PostDto>>>,
            BadRequest<ApiResponse<IEnumerable<PostDto>>>
        >
    > GetPosts(ClaimsPrincipal user, IPostRepository post_rep)
    {
        try
        {
            Guid UserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await post_rep.GetPosts(UserId);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<IEnumerable<PostDto>>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later",
                    Data = []
                }
            );
        }
    }

    public async Task<
        Results<
            Ok<ApiResponse<IEnumerable<PostDto>>>,
            BadRequest<ApiResponse<IEnumerable<PostDto>>>
        >
    > GetTrendingPosts(ClaimsPrincipal user, IPostRepository post_rep)
    {
        try
        {
            Guid UserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await post_rep.GetPosts(UserId);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<IEnumerable<PostDto>>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }

    public async Task<Results<Ok<ApiResponse<string>>, BadRequest<ApiResponse<string>>>> DeletePost(
        [FromRoute] Guid postid,
        IPostRepository post_rep
    )
    {
        try
        {
            var result = await post_rep.DeletePost(postid);
            return result.IsSuccess ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
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

    public async Task<Results<Ok<ApiResponse<string>>, BadRequest<ApiResponse<string>>>> Like(
        [FromBody] LikedPostDto likedPost,
        ClaimsPrincipal user,
        IPostRepository post_rep
    )
    {
        try
        {
            Guid user_id = Guid.Parse(user.FindFirst("Sub")!.Value);
            var result = await post_rep.UpdateLike(likedPost.PostId, user_id);
            return result.IsSuccess ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
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
        Results<Ok<ApiResponse<PostDetailDto>>, BadRequest<ApiResponse<PostDetailDto>>>
    > GetPost([FromRoute] Guid postid, ClaimsPrincipal user, IPostRepository post_rep)
    {
        try
        {
            Guid user_id = Guid.Parse(user.FindFirst("Sub")!.Value);
            var result = await post_rep.GetPost(postid, user_id);
            return result.IsSuccess ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<PostDetailDto>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }
}
