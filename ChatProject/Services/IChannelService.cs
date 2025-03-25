using ChatProject.Models;
using ChatProject.Models.ChatChannelModels;

namespace ChatProject.Services;

public interface IChannelService
{
    Task<ChatChannel?> GetChannelByIdAsync(string id);
    Task<List<ChatChannelDto>> GetAllChannelsAsync();
    Task<List<string>> GetAllUserChannelIdsAsync(string userId);
    Task<List<ChatChannelDto>> GetAllUserChannelsAsync(string userId);
    Task<ChatChannelDto> AddChannelAsync(string userId, string channelName);
    Task AddMessageToChannelAsync(string id, ChatMessage chatMessage);
    Task AddMemberToChannelAsync(string channelId, string userId);
    Task AddAdminToChannelAsync(string channelId, string userId);
    Task RemoveUserFromChannelAsync(string channelId, string userId);
}