using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;

namespace ChatProject.Helpers;

public class ModelConverter
{
    public static ChatUserDto UserBoToDto(ChatUser userBo, List<ChatChannelDto> channels, IEnumerable<ChatUser> friends)
    {
        
        return new ChatUserDto() 
        { 
            UserName = userBo.UserName!, 
            Channels = channels, 
            Friends = friends.Select(user => ChatUserToPersonDto(user)).ToList()
        };
    }

    public static ChatChannelDto ChannelBoToDto(ChatChannel channel)
    {
        return new ChatChannelDto
        {
            Id = channel.Id, 
            Name = channel.Name, 
            Admins = channel.Admins.Select(ChatUserToPersonDto).ToList(),
            Members = channel.Members.Select(ChatUserToPersonDto).ToList(),
            Owner = ChatUserToPersonDto(channel.CreatedBy),
            ChannelMessages = channel.ChannelMessages
        };
    }

    public static PersonDto ChatUserToPersonDto(ChatUser user)
    {
        return new PersonDto { UserName = user.UserName, UserId = user.Id};
    }
}