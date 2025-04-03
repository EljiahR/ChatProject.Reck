using ChatProject.Models.ChatUserModels;
using ChatProject.Models.JoinModels;
using ChatProject.Repositories;

namespace ChatProject.Services;

public class ChatUserService : IChatUserService
{
    private readonly IChatUserRepository _repository;

    public ChatUserService(IChatUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ChatUser?> GetUserWithChannelsByIdAsync(string userId)
    {
        return await _repository.GetUserWithChannelsByIdAsync(userId);
    }

    public async Task<ChatUserDto?> GetUserDtoAsync(string userId)
    {
        return await _repository.GetUserDtoAsync(userId);
    }

    public async Task<PersonDto?> ConfirmFriendAsync(string initiatorId, string receiverId)
    {
        return await _repository.ConfirmFriendAsync(initiatorId, receiverId);
    }

    public async Task<FriendshipDto?> RequestFriendAsync(string initiatorId, string receiverId)
    {
        return await _repository.RequestFriendAsync(initiatorId, receiverId);
    }
    
}