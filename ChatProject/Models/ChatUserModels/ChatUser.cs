using ChatProject.Models.JoinModels;
using Microsoft.AspNetCore.Identity;

namespace ChatProject.Models.ChatUserModels;

public class ChatUser : IdentityUser
{
    public override required string? UserName { get; set; }
    public override string? Email { get; set; }
    public List<ChannelUser> ChannelUsers { get; set; } = new();
    public List<Friendship> FriendsInitiated { get; set; } = new();
    public List<Friendship> FriendsReceived{ get; set; } = new();
}