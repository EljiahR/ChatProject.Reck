using ChatProject.Models;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ChatProject.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageService _service;

    public ChatHub(IMessageService service)
    {
        _service = service;
    }
    
    [Authorize(Policy = "BelongToChannel")]
    public async Task SendMessage(string user, string content, string channelName)
    {
        if (IsInChannel(channelName))
        {
            await _service.AddMessageAsync(new Message {Username = user, Content = content});
            await Clients.Group(channelName).SendAsync("ReceiveMessage", user, content, channelName);
        }

        await Clients.Caller.SendAsync("ReceiveMessage", "Server", "You are not a member of this channel");
    }

    public override async Task OnConnectedAsync()
    {
        var messages = _service.GetAllMessagesAsync();

        await Clients.Caller.SendAsync("ReceiveChatHistory", messages);
    }

    public bool IsInChannel(string channelName)
    {
        var user = Context.User;
        if (user == null) return false;

        var userChannels = user.Claims.Where(x => x.Type == "Channel").Select(x => x.Value).ToList();
        return userChannels.Contains(channelName);
    }
}