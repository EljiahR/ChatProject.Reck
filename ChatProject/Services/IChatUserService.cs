using ChatProject.Models.ChatUserModels;

namespace ChatProject.Services;

public interface IChatUserService
{
    Task<ChatUser?> GetUserWithChannelsByIdAsync(string userId);
}