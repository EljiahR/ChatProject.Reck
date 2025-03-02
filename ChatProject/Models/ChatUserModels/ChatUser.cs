using Microsoft.AspNetCore.Identity;

namespace ChatProject.Models.ChatUserModels;

public class ChatUser : IdentityUser
{
    public override required string? UserName { get; set; }
    public override string? Email { get; set; }
    public List<int> ChannelIds { get; set; } = new();
    public List<string> FriendIds { get; set; } = new();
}