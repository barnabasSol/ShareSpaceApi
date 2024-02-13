using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;

namespace ShareSpaceApi.Repository.Contracts;

public interface INotificationRepostiory
{
    void AddNotification(Guid source_id, Guid user_id, Status status);
    Task<ApiResponse<IEnumerable<NotificationsDto>>> GetNotifications(Guid user_id);
    Task<ApiResponse<int>> GetNotificationCount(Guid user_id);
}
