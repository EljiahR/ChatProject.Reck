using ChatProject.Models;
using ChatProject.Models.ChatChannelModels;

namespace ChatProject.Repositories;

public interface IChannelRepository
{
    Task<ChatChannel?> GetChannelByIdAsync(string id);
    Task<List<ChatChannelDto>> GetAllChannelsAsync();
    Task<List<string>> GetAllUserChannelIdsAsync(string userId);
    Task<List<ChatChannelDto>> GetAllUserChannelsAsync(string userId);
    Task<string> AddChannelAsync(ChatChannel newChannel);
    Task AddMessageToChannelAsync(string id, ChatMessage chatMessage);
    Task AddMemberToChannelAsync(string channelId, string userId);
    Task AddAdminToChannelAsync(string channelId, string userId);
}