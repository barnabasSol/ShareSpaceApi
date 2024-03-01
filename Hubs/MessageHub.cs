using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShareSpaceApi.Data.Context;
using ShareSpaceApi.Data.DTOs;
using ShareSpaceApi.Repository.Contracts;

namespace ShareSpaceApi.Hubs;

[Authorize(Roles = "user")]
public class MessageHub(IMessageRepository messageRepository, ShareSpaceDbContext shareSpaceDb)
    : Hub
{
    private readonly IMessageRepository messageRepository = messageRepository;
    private readonly ShareSpaceDbContext shareSpaceDb = shareSpaceDb;

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public async Task FetchMessagesOfUser(string username)
    {
        Guid current_user = Guid.Parse(
            Context.User!.Claims.FirstOrDefault(w => w.Type == "Sub")!.Value
        );
        var response = await messageRepository.GetMessagesOfUser(
            current_user,
            other_user: username
        );

        if (response.IsSuccess)
            await Clients.Caller.SendAsync("ShowMessages", response.Data);
    }

    public async Task GetUnseenMessagesCount()
    {
        Guid current_user = Guid.Parse(
            Context.User!.Claims.FirstOrDefault(w => w.Type == "Sub")!.Value
        );

        var response = await messageRepository.GetUnseenMessagesCount(current_user);
        if (response.IsSuccess)
            await Clients.Caller.SendAsync("ShowUnseenCount", response.Data);
    }

    public async Task SendMessageToUser(string username, string message)
    {
        var current_user = Context.User!.Claims.FirstOrDefault(_ => _.Type == "Sub")!.Value;
        var cur_user_obj = await shareSpaceDb.Users.FindAsync(Guid.Parse(current_user!));
        if (cur_user_obj is null)
        {
            return;
        }
        if (Context.UserIdentifier != username)
        {
            var response = await messageRepository.StoreMessage(
                new MessageDto
                {
                    From = Guid.Parse(current_user!),
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
                        Guid.Parse(current_user!),
                        cur_user_obj.ProfilePicUrl
                    );
                await Clients.Caller.SendAsync(
                    "ReceiveMessageFromUser",
                    Context.UserIdentifier,
                    message,
                    Guid.Parse(current_user!),
                    cur_user_obj.ProfilePicUrl
                );
            }
        }
    }

    public async Task ShowUsersInChat(string username)
    {
        var current_user = Context.User!.Claims.FirstOrDefault(_ => _.Type == "Sub")!.Value;

        var response = await messageRepository.GetUsersInChat(username);
        if (response.IsSuccess)
            await Clients.User(username).SendAsync("ReceiveUsersInChat", response.Data);
    }
}
