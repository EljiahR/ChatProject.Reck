using ChatProject.Models;

namespace ChatProject.Helpers;

public class ModelConverter
{
    public static ChatUserDto UserBoToDto(ChatUser userBo, IEnumerable<ChatChannel> channels)
    {
        
        return new ChatUserDto() { UserName = userBo.UserName, Channels = channels.Select(channel => ChannelBoToDto(channel)).ToList() };
    }

    public static ChatChannelDto ChannelBoToDto(ChatChannel channel)
    {
        return new ChatChannelDto() { Id = channel.Id, Name = channel.Name };
    }
}