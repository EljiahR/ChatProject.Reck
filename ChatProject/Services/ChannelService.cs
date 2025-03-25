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

    public async Task<ChatChannel?> GetChannelByIdAsync(string id)
    {
        return await _repository.GetChannelByIdAsync(id);
    }
    public async Task<List<ChatChannelDto>> GetAllChannelsAsync()
    {
        return await _repository.GetAllChannelsAsync();
    }

    public async Task<List<string>> GetAllUserChannelIdsAsync(string userId)
    {
        return await _repository.GetAllUserChannelIdsAsync(userId);
    }


    public async Task<List<ChatChannelDto>> GetAllUserChannelsAsync(string userId)
    {
        return await _repository.GetAllUserChannelsAsync(userId);
    }

    public async Task<ChatChannelDto> AddChannelAsync(string userId, string channelName)
    {
        var newChannel = await _repository.AddChannelAsync(userId, channelName);
        return newChannel;
    }

    public async Task AddMessageToChannelAsync(string id, ChatMessage chatMessage)
    {
        await _repository.AddMessageToChannelAsync(id, chatMessage);
    }

    public async Task AddMemberToChannelAsync(string channelId, string userId)
    {
        await _repository.AddMemberToChannelAsync(channelId, userId);
    }

    public async Task AddAdminToChannelAsync(string channelId, string userId)
    {
        await _repository.AddAdminToChannelAsync(channelId, userId);
    }

    public async Task RemoveUserFromChannelAsync(string channelId, string userId)
    {
        await _repository.RemoveUserFromChannelAsync(channelId, userId);
    }

    public async Task RemoveMessageFromChannelAsync(string channelId, string messageId)
    {
        await _repository.RemoveMessageFromChannelAsync(channelId, messageId);
    }

}