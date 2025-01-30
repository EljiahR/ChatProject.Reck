using ChatProject.Models;

namespace ChatProject.Services;

public interface IChannelService
{
    Task<IEnumerable<ChatChannel>> GetAllChannelsAsync();
    Task<IEnumerable<ChatChannel>> GetAllUserChannelsAsync(ChatUser user);
}