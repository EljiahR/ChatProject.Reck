using ChatProject.Models.ChatUserModels;
using ChatProject.Models.JoinModels;

namespace ChatProject.Repositories;

public interface IChatUserRepository
{
    Task<ChatUser?> GetUserWithChannelsByIdAsync(string userId);
    Task<ChatUserDto?> GetUserDtoAsync(string userId);
    Task<FriendshipDto?> ConfirmFriendAsync(string initiatorId, string receiverId);
    Task<FriendshipDto?> RequestFriendAsync(string initiatorId, string receiverId);
}