using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;

namespace ChatProject.Models.JoinModels;

public class ChannelUserDto
{
    public string UserId { get; set; }
    public PersonDto User { get; set; }
    
    public string ChannelId { get; set; }
    public ChatChannelDto Channel { get; set; }
    
    public ChannelRole Role { get; set; }
    public UserStatus Status { get; set; }
}