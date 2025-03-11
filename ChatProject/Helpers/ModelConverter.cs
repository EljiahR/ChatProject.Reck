using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;
using ChatProject.Models.JoinModels;

namespace ChatProject.Helpers;

public class ModelConverter
{
    public static ChatUserDto MapChatUserToDto(ChatUser userBo, List<ChatChannelDto> channels, IEnumerable<ChatUser> friends)
    {
        
        return new ChatUserDto() 
        { 
            Id = userBo.Id,
            UserName = userBo.UserName!, 
            Channels = channels, 
            Friends = friends.Select(MapChatUserToPersonDto).ToList()
        };
    }

    public static ChatChannelDto MapChannelToDto(ChatChannel channel)
    {
        return new ChatChannelDto
        {
            Id = channel.Id, 
            Name = channel.Name, 
            Admins = channel.ChannelUsers.Where(cu => cu.Role == ChannelRole.Admin).Select(cu => MapChatUserToPersonDto(cu.User)).ToList(),
            Members = channel.ChannelUsers.Where(cu => cu.Role == ChannelRole.Member).Select(cu => MapChatUserToPersonDto(cu.User)).ToList(),
            Owner = channel.ChannelUsers.Where(cu => cu.Role == ChannelRole.Creator).Select(cu => MapChatUserToPersonDto(cu.User)).FirstOrDefault()!,
            ChannelMessages = channel.ChannelMessages
        };
    }

    public static PersonDto MapChatUserToPersonDto(ChatUser user)
    {
        return new PersonDto { UserName = user.UserName, UserId = user.Id};
    }
    
}