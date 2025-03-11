using System.ComponentModel.DataAnnotations;
using ChatProject.Models.JoinModels;

namespace ChatProject.Models.ChatChannelModels;

public class ChatChannel
{
    [MaxLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string CreatedById { get; set; }
    public List<ChannelUser> ChannelUsers { get; set; } = new();
    public ICollection<ChatMessage> ChannelMessages { get; set; } = new List<ChatMessage>();
}