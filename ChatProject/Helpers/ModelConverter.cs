using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;
using ChatProject.Models.JoinModels;
// ReSharper disable All

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
            Channels = userBo.ChannelUsers.Select(cu => MapChannelToDto(cu.Channel, cu.Status)).ToList(), 
            Friends = friendList,
            FriendRequests = userBo.FriendsReceived.Where(f => f.Status == FriendshipStatus.Pending).ToList(),
            ChannelRequests = userBo.ChannelUsers.Where(cu => cu.Status == UserStatus.Pending).ToList()
        };
    }

    public static ChatChannelDto MapChannelToDto(ChatChannel channel, UserStatus userStatus)
    {
        return new ChatChannelDto
        {
            Id = channel.Id, 
            Name = channel.Name, 
            Admins = userStatus == UserStatus.Pending || userStatus == UserStatus.Banned ? [] : channel.ChannelUsers.Where(cu => cu.Role == ChannelRole.Admin).Select(cu => MapChatUserToPersonDto(cu.User)).ToList(),
            Members = userStatus == UserStatus.Pending || userStatus == UserStatus.Banned ? [] : channel.ChannelUsers.Where(cu => cu.Role == ChannelRole.Member).Select(cu => MapChatUserToPersonDto(cu.User)).ToList(),
            Owner = channel.ChannelUsers.Where(cu => cu.Role == ChannelRole.Creator).Select(cu => MapChatUserToPersonDto(cu.User)).FirstOrDefault()!,
            ChannelMessages = userStatus == UserStatus.Pending || userStatus == UserStatus.Banned ? [] : channel.ChannelMessages,
            IsPendingInvite = userStatus == UserStatus.Pending
        };
    }

    public static PersonDto MapChatUserToPersonDto(ChatUser user)
    {
        return new PersonDto { UserName = user.UserName, UserId = user.Id};
    }
    
}