using ChatProject.Models;
using ChatProject.Models.ChatChannelModels;

namespace ChatProject.Services;

public interface IChannelService
{
    Task<ChatChannel?> GetChannelByIdAsync(int id);
    Task<IEnumerable<ChatChannel>> GetAllChannelsAsync();
    Task<List<int>> GetAllUserChannelIdsAsync(string userId);
    Task<List<ChatChannelDto>> GetAllUserChannelsAsync(string userId);
    Task<int> AddChannelAsync(ChatChannel newChannel);
    Task AddMessageToChannelAsync(int id, ChatMessage chatMessage);
    Task AddMemberToChannelAsync(int channelId, string userId);
    Task AddAdminToChannelAsync(int channelId, string userId);
}