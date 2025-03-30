using ChatProject.Models;
using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.JoinModels;

namespace ChatProject.Services;

public interface IChannelService
{
    Task<ChatChannel?> GetChannelByIdAsync(string id);
    Task<List<ChatChannelDto>> GetAllChannelsAsync();
    Task<List<string>> GetAllUserChannelIdsAsync(string userId);
    Task<List<ChatChannelDto>> GetAllUserChannelsAsync(string userId);
    Task<ChatChannelDto> AddChannelAsync(string userId, string channelName);
    Task AddMessageToChannelAsync(string id, ChatMessage chatMessage);
    Task InviteUserToChannelAsync(string callerId, string channelId, string newUserId, ChannelRole role);
    Task ConfirmChannelInviteAsync(string channelId, string userId);
    Task RemoveUserFromChannelAsync(string channelId, string userId);
    Task RemoveMessageFromChannelAsync(string channelId, string messageId);

}