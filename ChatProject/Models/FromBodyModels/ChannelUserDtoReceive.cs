using ChatProject.Models.JoinModels;

namespace ChatProject.Models.FromBodyModels;

public class ChannelUserDtoReceive
{
    public string userId { get; set; }
    public string channelId { get; set; }
    public ChannelRole role { get; set; } = ChannelRole.Member;
}