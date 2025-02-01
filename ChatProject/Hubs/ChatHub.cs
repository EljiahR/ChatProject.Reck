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
    // Tracking what channels each user has access to without calling db several times, handles multiple connections from the same user
    private readonly ConcurrentDictionary<string, Dictionary<string, HashSet<int>>> _userConnections = new(); 

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
        
        await _channelService.AddMessageToChannelAsync(channelId, message);
        await Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", message);
        
    }
    
    // Adds user to all channel groups, handles multiple connections from the same user as well
    // No I did not come up with this myself
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        var connectionId = Context.ConnectionId;
        
        var user = await _userManager.GetUserAsync(Context.User!);
        var channelIds = user!.ChannelIds;

        _userConnections.AddOrUpdate(userId, 
            _ => new Dictionary<string, HashSet<int>> {{connectionId, new HashSet<int>(channelIds)}},
            (_, connections) =>
            {
                connections[connectionId] = new HashSet<int>(channelIds);
                return connections;
            }
        );

        foreach (var channelId in channelIds)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channelId.ToString());
        }

        await base.OnConnectedAsync();
    }

    // Handles disconnects for a user based on connection, removes user if all connections are disconnected
    // This does not handle irregular disconnects
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        var connectionId = Context.ConnectionId;

        if (_userConnections.TryGetValue(userId!, out var connections))
        {
            connections.Remove(connectionId);

            if (connections.Count == 0)
            {
                _userConnections.TryRemove(userId!, out _);
            }
        }
        
        return base.OnDisconnectedAsync(exception);
    }

    private bool IsInChannel(int channelId)
    {
        var userId = Context.UserIdentifier;
        var connectionId = Context.ConnectionId;
        return _userConnections.TryGetValue(userId!, out var connections) && connections[connectionId].Contains(channelId);

    }
    
}