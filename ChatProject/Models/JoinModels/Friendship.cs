using ChatProject.Models.ChatUserModels;

namespace ChatProject.Models.JoinModels;

public class Friendship
{
    public string InitiatorId { get; set; }
    public string ReceiverId { get; set; }
    
    public ChatUser Initiator { get; set; }
    public ChatUser Receiver { get; set; }
    
    public FriendshipStatus Status { get; set; }
}

public enum FriendshipStatus
{
    Friends,
    Pending,
    Blocked
}