using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;

namespace ChatProject.Helpers;

public class ModelConverter
{
    public static ChatUserDto UserBoToDto(ChatUser userBo, IEnumerable<ChatChannel> channels, IEnumerable<ChatUser> friends)
    {
        
        return new ChatUserDto() 
        { 
            UserName = userBo.UserName!, 
            Channels = channels.Select(channel => ChannelBoToDto(channel)).ToList(), 
            Friends = friends.Select(user => ChatUserToPersonDto(user, true)).ToList()
        };
    }

    public static ChatChannelDto ChannelBoToDto(ChatChannel channel)
    {
        return new ChatChannelDto { Id = channel.Id, Name = channel.Name};
    }

    public static PersonDto ChatUserToPersonDto(ChatUser user, bool isFriend)
    {
        return new PersonDto { UserName = user.UserName, UserId = user.Id, IsFriend = isFriend};
    }
}