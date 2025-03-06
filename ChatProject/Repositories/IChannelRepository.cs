using System.Collections;
using ChatProject.Models;
using ChatProject.Models.ChatChannelModels;

namespace ChatProject.Repositories;

public interface IChannelRepository
{
    Task<ChatChannel?> GetChannelByIdAsync(int id);
    Task<IEnumerable<ChatChannel>> GetAllChannelsAsync();
    Task<IEnumerable<ChatChannelDto>> GetAllUserChannelsAsync(string userId);
    Task<int> AddChannelAsync(ChatChannel newChannel);
    Task AddMessageToChannelAsync(int id, ChatMessage chatMessage);
    Task AddMemberToChannelAsync(int channelId, string userId);
    Task AddAdminToChannelAsync(int channelId, string userId);
}