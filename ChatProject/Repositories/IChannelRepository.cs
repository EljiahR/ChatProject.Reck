using System.Collections;
using ChatProject.Models;

namespace ChatProject.Repositories;

public interface IChannelRepository
{
    Task<ChatChannel?> GetChannelByIdAsync(int id);
    Task<IEnumerable<ChatChannel>> GetAllChannelsAsync();
    Task<IEnumerable<ChatChannel>> GetAllUserChannelsAsync(ChatUser user);
    Task AddMessageToChannel(int id, Message message);
}