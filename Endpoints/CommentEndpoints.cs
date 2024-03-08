using System.Security.Claims;
using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;
using ShareSpaceApi.Repository.Contracts;

namespace ShareSpaceApi.Endpoints;

public class CommentEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("comment")
            .RequireAuthorization(options => options.RequireRole("user"));

        group.MapPost("add", AddComment);
        group.MapDelete("delete/{commentid:guid}", DeleteComment);
    }

    public async Task<Results<Ok<ApiResponse<Guid>>, BadRequest<ApiResponse<Guid>>>> AddComment(
        ClaimsPrincipal user,
        CommentAddDto new_comment,
        ICommentRepository commentRepository
    )
    {
        try
        {
            Guid UserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await commentRepository.AddComment(new_comment, UserId);
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<Guid>
            {
                IsSuccess = false,
                Message = "Something went wrong, try again later." + ex.Message,
            };
            return TypedResults.BadRequest(errorResponse);
        }
    }

    public async Task<
        Results<Ok<ApiResponse<string>>, BadRequest<ApiResponse<string>>>
    > DeleteComment(
        [FromRoute] Guid commentid,
        ClaimsPrincipal user,
        ICommentRepository commentRepository
    )
    {
        try
        {
            Guid UserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await commentRepository.DeleteComment(commentid);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<string>
            {
                IsSuccess = false,
                Message = "Something went wrong, try again later." + ex.Message,
            };
            return TypedResults.BadRequest(errorResponse);
        }
    }
}
