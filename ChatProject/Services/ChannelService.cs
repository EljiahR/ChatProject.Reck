using ChatProject.Models;
using ChatProject.Repositories;

namespace ChatProject.Services;

public class ChannelService : IChannelService
{
    private readonly IChannelRepository _repository;

    public ChannelService(IChannelRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<ChatChannel>> GetAllChannelsAsync()
    {
        return await _repository.GetAllChannelsAsync();
    }

    public async Task<IEnumerable<ChatChannel>> GetAllUserChannelsAsync(ChatUser user)
    {
        return await _repository.GetAllUserChannelsAsync(user);
    }
}