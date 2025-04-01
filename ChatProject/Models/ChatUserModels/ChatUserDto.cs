using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.JoinModels;

namespace ChatProject.Models.ChatUserModels;

public class ChatUserDto
{
    public required string Id { get; set; } 
    public required string UserName { get; set; }
    public required List<ChatChannelDto> Channels { get; set; }
    public required List<PersonDto> Friends { get; set; } = new();
    public required List<FriendshipDto> FriendRequests { get; set; } = new();
    public required List<ChannelUserDto> ChannelInvites { get; set; } = new();
}