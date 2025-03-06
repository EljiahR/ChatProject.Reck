using ChatProject.Models.ChatUserModels;

namespace ChatProject.Models.ChatChannelModels;

public class ChatChannel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ChatUser CreatedBy { get; set; }
    public string CreatedById { get; set; }
    public List<ChatUser> Admins { get; set; } = new();
    public List<ChatUser> Members { get; set; } = new();
    public ICollection<ChatMessage> ChannelMessages { get; set; } = new List<ChatMessage>();
}