using ChatProject.Helpers;
using ChatProject.Models;
using ChatProject.Models.ChatUserModels;
using ChatProject.Models.JoinModels;
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
    private readonly IChatUserService _userService;
    private readonly ConnectionManager _connectionManager;

    public ChatHub(IChannelService channelService, IMessageService messageService, UserManager<ChatUser> userManager, IChatUserService userService, ConnectionManager connectionManager)
    {
        _channelService = channelService;
        _messageService = messageService;
        _userManager = userManager;
        _userService = userService;
        _connectionManager = connectionManager;
    }
    
    public async Task SendMessage(string content, string channelId)
    {
        var userId = Context.UserIdentifier;
  
        if (Context.User == null || string.IsNullOrWhiteSpace(userId) || !_connectionManager.IsInChannel(userId, channelId))
        {
            throw new HubException("Unauthorized");
        }
        
        var user = await _userManager.GetUserAsync(Context.User);
        if (user == null)
        {
            throw new HubException("Unauthorized");
        }
        
        var message = new ChatMessage {Content = content, Username = user.UserName!, ChannelId = channelId, SentById = user.Id};

        await _channelService.AddMessageToChannelAsync(channelId, message);
        await Clients.Group(channelId).SendAsync("ReceiveMessage", message);
        
    }
    
    public async Task RemoveMessage(string channelId, string messageId)
    {
        var userId = Context.UserIdentifier;
  
        if (Context.User == null || string.IsNullOrWhiteSpace(userId) || !_connectionManager.IsInChannel(userId, channelId))
        {
            throw new HubException("Unauthorized");
        }
        
        var user = await _userManager.GetUserAsync(Context.User);
        if (user == null)
        {
            throw new HubException("Unauthorized");
        }

        var messageToDelete = await _messageService.GetMessageByIdAsync(messageId);

        if (messageToDelete == null)
        {
            throw new HubException("Message not found");
        }

        if (messageToDelete.SentById != userId)
        {
            throw new HubException("Only sender can delete their message");
        }

        await _channelService.RemoveMessageFromChannelAsync(channelId, messageId);
        await Clients.Group(channelId).SendAsync("DeleteMessage", messageId);
    }
    
    // Send/Receive Channel Invites
    public async Task SendChannelInvite(string channelId, string newUserId)
    {
        try
        {
            var invite = await _channelService.InviteUserToChannelAsync(Context.UserIdentifier!, channelId, newUserId, ChannelRole.Member);

            await Clients.User(newUserId).SendAsync("GetChannelInvite", invite);
        }
        catch (Exception ex)
        {
            throw new HubException("Error sending channel invite: " + ex);
        }
    }
    
    // Accept Channel Invites
    public async Task AcceptChannelInvite(string channelId)
    {
        try
        {
            var userId = Context.UserIdentifier!;
            var channelDto = await _channelService.ConfirmChannelInviteAsync(channelId, userId);
            _connectionManager.AddChannel(userId, channelId);
            var connections = _connectionManager.GetConnections(userId);
            foreach (var connection in connections)
            {
                await Groups.AddToGroupAsync(connection, channelId);
            }
            
            // Return new user dto to channel 
            await Clients.Group(channelId).SendAsync("ReceiveNewMember",
                new { channelId, user = channelDto.Members.FirstOrDefault(u => u.Id == userId) });
            
            await Clients.Caller.SendAsync("JoinChannel", channelDto);
        }
        catch (Exception ex)
        {
            throw new HubException("Error accepting channel invite: " + ex);
        }
    }
    
    // Send/Receive Friend Requests
    public async Task SendFriendRequest(string userId)
    {
        try
        {
            var request = await _userService.RequestFriendAsync(Context.UserIdentifier!, userId);
            if (request == null)
            {
                throw new HubException("Error sending friend request: ");
            }

            await Clients.User(userId).SendAsync("ReceiveFriendRequest", request);
        }
        catch (Exception ex)
        {
            throw new HubException("Error sending friend request: " + ex);
        }
    }
    
    // Accept Friend Requests
    public async Task AcceptFriendRequest(string initiatorId)
    {
        try
        {
            var friendship = await _userService.ConfirmFriendAsync(initiatorId, Context.UserIdentifier!);
            if (friendship == null)
            {
                throw new HubException("Error accepting friend request: ");
            }

            await Clients.Caller.SendAsync("ReceiveNewFriend", friendship.Initiator);
            await Clients.User(initiatorId).SendAsync("ReceiveNewFriend", friendship.Receiver);
        }
        catch (Exception ex)
        {
            throw new HubException("Error accepting friend request: " + ex);
        }
    }
    
    // Adds user to all channel groups, handles multiple connections from the same user as well
    // No I did not come up with this myself
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        var connectionId = Context.ConnectionId;

        _connectionManager.AddConnection(userId, connectionId);
        List<string> channelIds;
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
            await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
        }
        
        await base.OnConnectedAsync();
    }

    public async Task AfterConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        var userChannels = await _channelService.GetAllUserChannelsAsync(userId);
        var messageHistory = new Dictionary<string, List<ChatMessage>>();
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