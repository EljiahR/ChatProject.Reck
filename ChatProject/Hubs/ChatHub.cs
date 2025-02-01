using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ChatProject.Models;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ChatProject.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChannelService _channelService;
    private readonly UserManager<ChatUser> _userManager;
    private readonly ConcurrentDictionary<string, HashSet<int>> _userChannels = new(); // Tracking what channels each user has access to without calling db several times

    public ChatHub(IChannelService service, UserManager<ChatUser> userManager)
    {
        _channelService = service;
        _userManager = userManager;
    }
    
    public async Task SendMessage(string content, int channelId)
    {
        if (!IsInChannel(channelId))
        {
            throw new HubException("Unauthorized");
        }
        
        var user = await _userManager.GetUserAsync(Context.User!);
        var message = new Message {Content = content, Username = user!.UserName!};
        await Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", message);
        await _channelService.AddMessageToChannelAsync(channelId, message);
    }
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!; 
        
        var user = await _userManager.GetUserAsync(Context.User!);
        var channelIds = user!.ChannelIds;
        _userChannels[userId] = new HashSet<int>(channelIds);

        foreach (var channelId in channelIds)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channelId.ToString());
        }

        await base.OnConnectedAsync();
    }

    public bool IsInChannel(int channelId)
    {
        var userId = Context.UserIdentifier;
        return _userChannels.TryGetValue(userId!, out var channels) && channels.Contains(channelId);

    }
    
}