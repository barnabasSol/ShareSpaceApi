using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Data.DTOs.ResponseType;

namespace ShareSpaceApi.Repository.Contracts;

public interface IMessageRepository
{
    Task<ApiResponse<IEnumerable<MessageDto>>> GetMessagesOfUser(
        Guid current_user,
        string other_user
    );
    Task<ApiResponse<IEnumerable<UserMessageDto>>> GetUsersInChat(string username);
    Task<ApiResponse<string>> StoreMessage(MessageDto message);
    Task<ApiResponse<string>> DeleteMessage(Guid message_id);
    Task<ApiResponse<int>> GetUnseenMessagesCount(Guid current_user);
    Task<ApiResponse<string>> UpdateOnlineStatus(Guid current_user, bool status);
}
