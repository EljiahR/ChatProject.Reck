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
        friendList.AddRange(userBo.FriendsInitiated.Where(f => f.Status == FriendshipStatus.Friends).Select(f => MapChatUserToPersonDto(f.Receiver, true)));
        friendList.AddRange(userBo.FriendsReceived.Where(f => f.Status == FriendshipStatus.Friends).Select(f => MapChatUserToPersonDto(f.Initiator, true)));
        
        return new ChatUserDto() 
        { 
            Id = userBo.Id,
            UserName = userBo.UserName!, 
            Channels = userBo.ChannelUsers.Where(cu => cu.Status == UserStatus.Active).Select(cu => MapChannelToDto(cu.Channel, cu.Status)).ToList(), 
            Friends = friendList,
            FriendRequests = userBo.FriendsReceived.Where(f => f.Status == FriendshipStatus.Pending).Select(MapFriendshipToDto).ToList(),
            ChannelInvites = userBo.ChannelUsers.Where(cu => cu.Status == UserStatus.Pending).Select(MapChannelUserToDto).ToList()
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

    public static PersonDto MapChatUserToPersonDto(ChatUser user, bool isFriend = false)
    {
        return new PersonDto { UserName = user.UserName, Id = user.Id, IsFriend = isFriend};
    }

    public static FriendshipDto MapFriendshipToDto(Friendship friendship)
    {
        return new FriendshipDto
        {
            Id = friendship.Id,
            InitiatorId = friendship.InitiatorId,
            ReceiverId = friendship.ReceiverId,
            Initiator = MapChatUserToPersonDto(friendship.Initiator),
            Receiver = MapChatUserToPersonDto(friendship.Receiver)
        };
    }

    public static ChannelUserDto MapChannelUserToDto(ChannelUser channelUser)
    {
        return new ChannelUserDto
        {
            Id = channelUser.Id,
            User = MapChatUserToPersonDto(channelUser.User),
            UserId = channelUser.UserId,
            Channel = MapChannelToDto(channelUser.Channel, channelUser.Status),
            ChannelId = channelUser.ChannelId,
            Role = channelUser.Role,
            Status = channelUser.Status
        };
    }
    
}