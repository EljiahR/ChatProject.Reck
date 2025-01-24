using ChatProject.Models;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
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
    public async Task SendMessage(string user, string content)
    {
        await _service.AddMessageAsync(new Message {Username = user, Content = content});
        await Clients.All.SendAsync("ReceiveMessage", user, content);
    }

    public override async Task OnConnectedAsync()
    {
        var messages = _service.GetAllMessagesAsync();

        await Clients.Caller.SendAsync("ReceiveChatHistory", messages);
    }
}