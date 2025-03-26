using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;

namespace ChatProject.Models.JoinModels;

public class ChannelUser
{
    public string UserId { get; set; }
    public ChatUser User { get; set; }
    
    public string ChannelId { get; set; }
    public ChatChannel Channel { get; set; }
    
    public ChannelRole Role { get; set; }
    public UserStatus State { get; set; }
}

public enum ChannelRole
{
    Creator,
    Admin,
    Member
}

public enum UserStatus
{
    Pending,
    Active,
    Banned
}