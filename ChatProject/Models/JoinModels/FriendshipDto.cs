using ChatProject.Models.ChatUserModels;

namespace ChatProject.Models.JoinModels;

public class FriendshipDto
{
    public string InitiatorId { get; set; }
    public string ReceiverId { get; set; }
    
    public PersonDto Initiator { get; set; }
    public PersonDto Receiver { get; set; }
    
    public FriendshipStatus Status { get; set; }
}