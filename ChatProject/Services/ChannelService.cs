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

    public async Task<ChatChannelDto?> GetChannelByIdAsync(string id)
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

    public async Task<ChannelUserDto> InviteUserToChannelAsync(string callerId, string channelId, string userId, ChannelRole role)
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
            return await _repository.InviteMemberToChannelAsync(channelId, userId);
        }
        
        return await  _repository.InviteMemberToChannelAsync(channelId, userId);
        
    }
    public async Task<ChatChannelDto> ConfirmChannelInviteAsync(string channelId, string userId)
    {
        var user = await _userRepository.GetUserWithChannelsByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User was not found.");
        }

        var channelInvite = user.ChannelUsers.FirstOrDefault(cu => cu.ChannelId == channelId);
        if (channelInvite == null || channelInvite.Status != UserStatus.Pending)
        {
            throw new InvalidOperationException("Invite not found.");
        }
        
        return await _repository.ConfirmChannelInviteAsync(channelId, userId);
    }

    public async Task RemoveUserFromChannelAsync(string channelId, string userId)
    {
        await _repository.RemoveUserFromChannelAsync(channelId, userId);
    }

    public async Task RemoveMessageFromChannelAsync(string channelId, string messageId)
    {
        await _repository.RemoveMessageFromChannelAsync(channelId, messageId);
    }

    public async Task UpdateChannelAsync(UpdateChatChannel channelUpdate)
    {
        await _repository.UpdateChannelAsync(channelUpdate);
    }

}