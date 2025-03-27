using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.JoinModels;

namespace ChatProject.Models.ChatUserModels;

public class ChatUserDto
{
    public required string Id { get; set; } 
    public required string UserName { get; set; }
    public required List<ChatChannelDto> Channels { get; set; }
    public required List<PersonDto> Friends { get; set; }
    public required List<Friendship> FriendRequests { get; set; }
    public required List<ChatChannel> ChannelRequests { get; set; }
}