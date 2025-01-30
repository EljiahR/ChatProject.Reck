using System.Collections;
using ChatProject.Models;

namespace ChatProject.Repositories;

public interface IChannelRepository
{
    Task<IEnumerable<ChatChannel>> GetAllChannelsAsync();
    Task<IEnumerable<ChatChannel>> GetAllUserChannelsAsync(ChatUser user);

}