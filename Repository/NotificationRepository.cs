using Microsoft.EntityFrameworkCore;
using ShareSpaceApi.Data.Context;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;
using ShareSpaceApi.Data.Models;
using ShareSpaceApi.Repository.Contracts;

namespace ShareSpaceApi.Repository;

public class NotificationRepository(ShareSpaceDbContext shareSpaceDb) : INotificationRepostiory
{
    private readonly ShareSpaceDbContext shareSpaceDb = shareSpaceDb;

    public async void AddNotification(Guid source_id, Guid user_id, Status status)
    {
        try
        {
            await shareSpaceDb.Notifications.AddAsync(
                new Notification
                {
                    SourceId = source_id,
                    UserId = user_id,
                    Type = (int)status
                }
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"failed to add notification, {ex.Message}");
        }
    }

    public async Task<ApiResponse<int>> GetNotificationCount(Guid user_id)
    {
        try
        {
            var count = await shareSpaceDb.Notifications
                .Where(_ => _.UserId == user_id)
                .CountAsync();

            return new ApiResponse<int> { IsSuccess = true, Data = count };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ApiResponse<IEnumerable<NotificationsDto>>> GetNotifications(Guid user_id)
    {
        try
        {
            var notifications = await shareSpaceDb.Notifications
                .Include(i => i.UserSource)
                .Where(_ => _.UserId == user_id)
                .ToListAsync();

            return new ApiResponse<IEnumerable<NotificationsDto>>
            {
                IsSuccess = true,
                Data = notifications
                    .Select(
                        s =>
                            new NotificationsDto
                            {
                                UserName = s.UserSource!.UserName,
                                Name = s.UserSource!.Name,
                                CreatedAt = s.CreatedAt,
                                ProfilePicUrl = s.UserSource!.ProfilePicUrl,
                                Status = s.Type == 1 ? Status.Followed : Status.Unfollowed
                            }
                    )
                    .OrderByDescending(o => o.CreatedAt)
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}