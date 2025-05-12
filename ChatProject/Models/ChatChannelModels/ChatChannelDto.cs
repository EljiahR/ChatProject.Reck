using ChatProject.Models.ChatUserModels;

namespace ChatProject.Models.ChatChannelModels;

public class ChatChannelDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public PersonDto Owner { get; set; }
    public List<PersonDto> Admins { get; set; } = new();
    public List<PersonDto> Members { get; set; } = new();
    public ICollection<ChatMessage> ChannelMessages { get; set; } = new List<ChatMessage>();
    public bool IsPendingInvite { get; set; } = false;
    public bool IsFrozen { get; set; } = false;
}