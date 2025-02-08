using System.Collections;
using ChatProject.Models;

namespace ChatProject.Repositories;

public interface IChannelRepository
{
    Task<ChatChannel?> GetChannelByIdAsync(int id);
    Task<IEnumerable<ChatChannel>> GetAllChannelsAsync();
    Task<IEnumerable<ChatChannel>> GetAllUserChannelsAsync(string userId);
    Task<int> AddChannelAsync(ChatChannel newChannel);
    Task AddMessageToChannelAsync(int id, ChatMessage chatMessage);
}