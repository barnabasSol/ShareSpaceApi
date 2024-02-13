using System.Security.Claims;
using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;
using ShareSpaceApi.Repository.Contracts;

namespace ShareSpaceApi.Endpoints;

public class NotificaionEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("notification")
            .RequireAuthorization(options => options.RequireRole("user"));

        group.MapGet("", GetNotifications);
        group.MapGet("count", GetNotificationsCount);
    }

    public static async Task<
        Results<Ok<ApiResponse<int>>, BadRequest<ApiResponse<int>>>
    > GetNotificationsCount(ClaimsPrincipal user, INotificationRepostiory notif_rep)
    {
        try
        {
            Guid UserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await notif_rep.GetNotificationCount(UserId);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }

    public static async Task<
        Results<
            Ok<ApiResponse<IEnumerable<NotificationsDto>>>,
            BadRequest<ApiResponse<IEnumerable<NotificationsDto>>>
        >
    > GetNotifications(ClaimsPrincipal user, INotificationRepostiory notif_rep)
    {
        try
        {
            Guid UserId = Guid.Parse(user.FindFirst("Sub")!.Value);
            var response = await notif_rep.GetNotifications(UserId);
            return response.IsSuccess
                ? TypedResults.Ok(response)
                : TypedResults.BadRequest(response);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(
                new ApiResponse<IEnumerable<NotificationsDto>>
                {
                    IsSuccess = false,
                    Message = $"server error happened, {ex.Message}. try again later"
                }
            );
        }
    }
}
