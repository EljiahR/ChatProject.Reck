using System.Text.Json.Serialization;

namespace ChatProject.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; } = DateTime.Now;
    public int ChannelId { get; set; }
}