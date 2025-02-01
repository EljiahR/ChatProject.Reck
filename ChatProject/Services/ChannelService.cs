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

    public async Task<ChatChannel?> GetChannelByIdAsync(int id)
    {
        return await _repository.GetChannelByIdAsync(id);
    }
    public async Task<IEnumerable<ChatChannel>> GetAllChannelsAsync()
    {
        return await _repository.GetAllChannelsAsync();
    }

    public async Task<IEnumerable<ChatChannel>> GetAllUserChannelsAsync(string userId)
    {
        return await _repository.GetAllUserChannelsAsync(userId);
    }

    public async Task AddChannelAsync(ChatChannel newChannel)
    {
        await _repository.AddChannelAsync(newChannel);
    }

    public async Task AddMessageToChannelAsync(int id, Message message)
    {
        await _repository.AddMessageToChannelAsync(id, message);
    }
}