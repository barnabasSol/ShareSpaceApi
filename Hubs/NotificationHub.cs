using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ShareSpaceApi.Hubs;

public class NotificationHub : Hub
{
    [Authorize(Roles = "user")]
    public async Task NotifyUserMessage(string username, string message)
    {
        await Clients
            .User(username)
            .SendAsync("ReceiveNotificationFromUser", Context.UserIdentifier, message);
    }
}
