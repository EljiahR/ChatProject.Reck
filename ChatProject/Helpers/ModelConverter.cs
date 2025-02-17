using ChatProject.Models;

namespace ChatProject.Helpers;

public class ModelConverter
{
    public static ChatUserDto UserBoToDto(ChatUser userBo, IEnumerable<ChatChannel> channels, IEnumerable<ChatUser> friends)
    {
        
        return new ChatUserDto() 
        { 
            UserName = userBo.UserName, 
            Channels = channels.Select(channel => ChannelBoToDto(channel)).ToList(), 
            Friends = friends.Select(user => ChatUserToFriendDto(user)).ToList()
        };
    }

    private static ChatChannelDto ChannelBoToDto(ChatChannel channel)
    {
        return new ChatChannelDto() { Id = channel.Id, Name = channel.Name };
    }

    private static FriendDto ChatUserToFriendDto(ChatUser user)
    {
        return new FriendDto() { UserName = user.UserName, UserId = user.Id };
    }
}