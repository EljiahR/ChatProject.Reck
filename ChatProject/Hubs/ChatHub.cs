using ChatProject.Helpers;
using ChatProject.Models;
using ChatProject.Models.ChatUserModels;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ChatProject.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChannelService _channelService;
    private readonly IMessageService _messageService;
    private readonly UserManager<ChatUser> _userManager;
    private readonly ConnectionManager _connectionManager;

    public ChatHub(IChannelService channelService, IMessageService messageService, UserManager<ChatUser> userManager, ConnectionManager connectionManager)
    {
        _channelService = channelService;
        _messageService = messageService;
        _userManager = userManager;
        _connectionManager = connectionManager;
    }
    
    public async Task SendMessage(string content, string channelId)
    {
        var userId = Context.UserIdentifier;
        if (!_connectionManager.IsInChannel(userId!, channelId))
        {
            throw new HubException("Unauthorized");
        }
        
        var user = await _userManager.GetUserAsync(Context.User!);
        var message = new ChatMessage {Content = content, Username = user!.UserName!, ChannelId = channelId};

        await _channelService.AddMessageToChannelAsync(channelId, message);
        await Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", message);
        
    }
    
    // Adds user to all channel groups, handles multiple connections from the same user as well
    // No I did not come up with this myself
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        var connectionId = Context.ConnectionId;

        _connectionManager.AddConnection(userId, connectionId);
        List<int> channelIds;
        if (_connectionManager.GetConnections(userId).Count == 1)
        {

            channelIds = await _channelService.GetAllUserChannelIdsAsync(userId);
            _connectionManager.AddChannels(userId, channelIds);
        }
        else
        {
            channelIds = _connectionManager.GetChannels(userId);
        }
        
        foreach (var channelId in channelIds)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channelId.ToString());
        }
        
        await base.OnConnectedAsync();
    }

    public async Task AfterConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        var userChannels = await _channelService.GetAllUserChannelsAsync(userId);
        var messageHistory = new Dictionary<int, List<ChatMessage>>();
        foreach (var channel in userChannels)
        {
            messageHistory[channel.Id] = channel.ChannelMessages.ToList();
        }

        await Clients.Caller.SendAsync("ReceiveMessageHistory", messageHistory);
    }

    // Handles disconnects for a user based on connection, removes user if all connections are disconnected
    // This does not handle irregular disconnects
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        var connectionId = Context.ConnectionId;
        
        _connectionManager.RemoveConnection(userId!, connectionId);
        
        return base.OnDisconnectedAsync(exception);
    }
}