using ChatProject.Models;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ChatProject.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChannelService _service;
    private readonly UserManager<ChatUser> _userManager;

    public ChatHub(IChannelService service, UserManager<ChatUser> userManager)
    {
        _service = service;
        _userManager = userManager;
    }
    
    [Authorize(Policy = "BelongToChannel")]
    public async Task SendMessage(string content, int channelId)
    {
        if (await IsInChannel(channelId))
        {
            var user = await _userManager.GetUserAsync(Context.User!);
            
            await _service.AddMessageToChannelAsync(channelId, new Message {Username = user!.UserName!, Content = content});
            await Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", user, content, channelId);
        }

        await Clients.Caller.SendAsync("ReceiveMessage", "Server", "You are not a member of this channel");
    }
    
    [Authorize(Policy = "BelongToChannel")]
    public async Task AddToChannel(int channelId)
    {
        if (await IsInChannel(channelId))
        {
            var channel = await _service.GetChannelByIdAsync(channelId);
            await Groups.AddToGroupAsync(Context.ConnectionId, channelId.ToString());
            
            await Clients.Caller.SendAsync("ReceiveChatHistory", channel!.Messages);
        }
    }

    public async Task RemoveFromChannel(int channelId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId.ToString());
    }

    public async Task<bool> IsInChannel(int channelId)
    {
        var user = await _userManager.GetUserAsync(Context.User!);
        if (user == null) return false;
        
        return user.ChannelIds.Contains(channelId);
    }
}