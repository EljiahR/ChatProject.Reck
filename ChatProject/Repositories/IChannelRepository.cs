using ChatProject.Models;
using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;
using ChatProject.Models.JoinModels;

namespace ChatProject.Repositories;

public interface IChannelRepository
{
    Task<ChatChannelDto?> GetChannelByIdAsync(string id, bool withoutIncludes);
    Task<List<ChatChannelDto>> GetAllChannelsAsync();
    Task<List<string>> GetAllUserChannelIdsAsync(string userId);
    Task<List<ChatChannelDto>> GetAllUserChannelsAsync(string userId);
    Task<ChatChannelDto> AddChannelAsync(string userId, string channelName);
    Task AddMessageToChannelAsync(string id, ChatMessage chatMessage);
    Task<ChannelUserDto> InviteMemberToChannelAsync(string channelId, string userId);
    Task<ChannelUserDto> InviteAdminToChannelAsync(string channelId, string userId);
    Task<ChatChannelDto> ConfirmChannelInviteAsync(string channelId, string userId);
    Task RemoveUserFromChannelAsync(string channelId, string userId);
    Task RemoveMessageFromChannelAsync(string channelId, string messageId);
    Task UpdateChannelAsync(UpdateChatChannel channelUpdate);
}