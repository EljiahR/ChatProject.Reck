using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;
using ChatProject.Models.JoinModels;
using NuGet.Packaging;

namespace ChatProject.Helpers;

public class ModelConverter
{
    public static ChatUserDto MapChatUserToDto(ChatUser userBo)
    {
        List<PersonDto> friendList = new();
        friendList.AddRange(userBo.FriendsInitiated.Select(f => MapChatUserToPersonDto(f.Receiver)));
        friendList.AddRange(userBo.FriendsReceived.Select(f => MapChatUserToPersonDto(f.Initiator)));
        
        return new ChatUserDto() 
        { 
            Id = userBo.Id,
            UserName = userBo.UserName!, 
            Channels = userBo.ChannelUsers.Select(cu => MapChannelToDto(cu.Channel)).ToList(), 
            Friends = friendList
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