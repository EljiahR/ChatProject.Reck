using ChatProject.Models.ChatChannelModels;
using Microsoft.AspNetCore.Identity;

namespace ChatProject.Models.ChatUserModels;

public class ChatUser : IdentityUser
{
    public override required string? UserName { get; set; }
    public override string? Email { get; set; }
    public List<ChatChannel> CreatedChannels { get; set; } = new();
    public List<ChatChannel> AdministeredChannels { get; set; } = new();
    public List<ChatChannel> MemberChannels { get; set; } = new();
    public List<string> FriendIds { get; set; } = new();
}