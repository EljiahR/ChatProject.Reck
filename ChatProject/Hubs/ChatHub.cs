using ChatProject.Models;
using ChatProject.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatProject.Hubs;

public class ChatHub : Hub
{
    private readonly IMessageService _service;

    public ChatHub(IMessageService service)
    {
        _service = service;
    }
    public async Task SendMessage(string user, string content)
    {
        _service.AddMessage(new Message {Username = user, Content = content});
        await Clients.All.SendAsync("ReceiveMessage", user, content);
    }
}