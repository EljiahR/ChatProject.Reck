using ChatProject.Models.ChatUserModels;
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

    public async Task AddFriends(string initiatorId, string receiverId)
    { 
        await _repository.AddFriends(initiatorId, receiverId);
    }
    
}