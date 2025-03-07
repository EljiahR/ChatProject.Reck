using ChatProject.Models;
using ChatProject.Models.ChatChannelModels;
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
    public async Task<List<ChatChannelDto>> GetAllChannelsAsync()
    {
        return await _repository.GetAllChannelsAsync();
    }

    public async Task<List<int>> GetAllUserChannelIdsAsync(string userId)
    {
        return await _repository.GetAllUserChannelIdsAsync(userId);
    }


    public async Task<List<ChatChannelDto>> GetAllUserChannelsAsync(string userId)
    {
        return await _repository.GetAllUserChannelsAsync(userId);
    }

    public async Task<int> AddChannelAsync(ChatChannel newChannel)
    {
        await _repository.AddChannelAsync(newChannel);
        return newChannel.Id;
    }

    public async Task AddMessageToChannelAsync(int id, ChatMessage chatMessage)
    {
        await _repository.AddMessageToChannelAsync(id, chatMessage);
    }

    public async Task AddMemberToChannelAsync(int channelId, string userId)
    {
        await _repository.AddMemberToChannelAsync(channelId, userId);
    }

    public async Task AddAdminToChannelAsync(int channelId, string userId)
    {
        await _repository.AddAdminToChannelAsync(channelId, userId);
    }
}