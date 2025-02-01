using ChatProject.Models;

namespace ChatProject.Services;

public interface IChannelService
{
    Task<ChatChannel?> GetChannelByIdAsync(int id);
    Task<IEnumerable<ChatChannel>> GetAllChannelsAsync();
    Task<IEnumerable<ChatChannel>> GetAllUserChannelsAsync(ChatUser user);
    Task AddChannelAsync(ChatChannel newChannel);
    Task AddMessageToChannelAsync(int id, Message message);
}