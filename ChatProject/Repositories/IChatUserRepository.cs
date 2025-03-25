using ChatProject.Models.ChatUserModels;

namespace ChatProject.Repositories;

public interface IChatUserRepository
{
    Task<ChatUser?> GetUserWithChannelsByIdAsync(string userId);
    Task<ChatUserDto?> GetUserDtoAsync(string userId);
}