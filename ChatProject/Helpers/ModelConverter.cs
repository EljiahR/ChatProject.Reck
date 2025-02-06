using ChatProject.Models;

namespace ChatProject.Helpers;

public class ModelConverter
{
    public static ChatUserDto UserBoToDto(ChatUser userBo)
    {
        return new ChatUserDto() { UserName = userBo.UserName, ChannelIds = userBo.ChannelIds };
    }
}