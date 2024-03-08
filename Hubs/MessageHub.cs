using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShareSpaceApi.Data.Context;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Hubs.ResponseTypes;
using ShareSpaceApi.Repository.Contracts;

namespace ShareSpaceApi.Hubs;

[Authorize(Roles = "user")]
public class MessageHub(IMessageRepository messageRepository, ShareSpaceDbContext shareSpaceDb)
    : Hub
{
    private readonly IMessageRepository messageRepository = messageRepository;
    private readonly ShareSpaceDbContext shareSpaceDb = shareSpaceDb;

    public override async Task OnConnectedAsync()
    {
        Guid connected_user_id = Guid.Parse(Context.User!.FindFirst("Sub")!.Value);
        var status = await messageRepository.UpdateOnlineStatus(connected_user_id, true);
        if (status.IsSuccess)
            await Clients.All.SendAsync(
                "UserOnlineStatusChanged",
                new OnlineStatus(connected_user_id, true)
            );
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Guid disconnected_user_id = Guid.Parse(Context.User!.FindFirst("Sub")!.Value);
        var status = await messageRepository.UpdateOnlineStatus(disconnected_user_id, false);
        if (status.IsSuccess)
            await Clients.All.SendAsync(
                "UserOnlineStatusChanged",
                new OnlineStatus(disconnected_user_id, false)
            );
    }

    public async Task FetchMessagesOfUser(string username)
    {
        var current_user = Guid.Parse(Context.User!.FindFirst("Sub")!.Value);

        var response = await messageRepository.GetMessagesOfUser(
            current_user,
            other_user: username
        );

        if (response.IsSuccess)
            await Clients.Caller.SendAsync("ShowMessages", response.Data);
    }

    public async Task GetUnseenMessagesCount()
    {
        var current_user = Guid.Parse(Context.User!.FindFirst("Sub")!.Value);

        var response = await messageRepository.GetUnseenMessagesCount(current_user);
        if (response.IsSuccess)
            await Clients.Caller.SendAsync("ShowUnseenCount", response.Data);
    }

    public async Task SendMessageToUser(string username, string message)
    {
        var sending_user = Guid.Parse(Context.User!.FindFirst("Sub")!.Value);
        var cur_user_obj = await shareSpaceDb.Users.FindAsync(sending_user);

        if (cur_user_obj is null)
            return;

        if (Context.UserIdentifier != username)
        {
            var response = await messageRepository.StoreMessage(
                new MessageDto
                {
                    From = sending_user,
                    To = username,
                    Text = message,
                }
            );

            if (response.IsSuccess)
            {
                await Clients
                    .User(username)
                    .SendAsync(
                        "ReceiveMessageFromUser",
                        Context.UserIdentifier,
                        message,
                        sending_user,
                        cur_user_obj.ProfilePicUrl
                    );
                await Clients.Caller.SendAsync(
                    "ReceiveMessageFromUser",
                    Context.UserIdentifier,
                    message,
                    sending_user,
                    cur_user_obj.ProfilePicUrl
                );
            }
        }
    }

    public async Task ShowUsersInChat(string username)
    {
        // var current_user = Context.User!.FindFirst("Sub")!.Value;
        var response = await messageRepository.GetUsersInChat(username);
        if (response.IsSuccess)
            await Clients.User(username).SendAsync("ReceiveUsersInChat", response.Data);
    }
}
