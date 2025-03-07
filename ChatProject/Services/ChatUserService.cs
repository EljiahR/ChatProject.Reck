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
    
}