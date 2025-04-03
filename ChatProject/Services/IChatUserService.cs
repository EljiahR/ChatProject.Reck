using ChatProject.Models.ChatUserModels;

namespace ChatProject.Services;

public interface IChatUserService
{
    Task<ChatUser?> GetUserWithChannelsByIdAsync(string userId);
    Task<ChatUserDto?> GetUserDtoAsync(string userId);
    Task ConfirmFriendAsync(string initiatorId, string receiverId);
    Task RequestFriendAsync(string initiatorId, string receiverId);

}