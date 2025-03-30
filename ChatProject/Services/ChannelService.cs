using ChatProject.Models;
using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.JoinModels;
using ChatProject.Repositories;

namespace ChatProject.Services;

public class ChannelService : IChannelService
{
    private readonly IChannelRepository _repository;
    private readonly IChatUserRepository _userRepository;

    public ChannelService(IChannelRepository repository, IChatUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
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

    public async Task InviteUserToChannelAsync(string callerId, string channelId, string userId, ChannelRole role)
    {
        var user = await _userRepository.GetUserWithChannelsByIdAsync(callerId);
        if (user == null)
        {
            throw new InvalidOperationException("User was not found.");
        }
        
        if (user.ChannelUsers.All(cu => cu.ChannelId != channelId))
        {
            throw new InvalidOperationException("User not a member of the channel.");
        }

        var newChannelUser = await _userRepository.GetUserWithChannelsByIdAsync(userId);

        if (newChannelUser == null)
        {
            throw new InvalidOperationException($"User with id '{userId}' was not found.");
        }
        
        if (newChannelUser.ChannelUsers.Any(cu => cu.ChannelId == channelId))
        {
            throw new InvalidOperationException("User already in channel.");
        }

        if (role == ChannelRole.Admin)
        {
            await _repository.InviteMemberToChannelAsync(channelId, userId);
        }
        else
        {
            await  _repository.InviteMemberToChannelAsync(channelId, userId);
        }
    }
    public async Task ConfirmChannelInviteAsync(string channelId, string userId)
    {
        await _repository.ConfirmChannelInviteAsync(channelId, userId);
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