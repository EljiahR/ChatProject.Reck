using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChatProject.Models;

public class ChatMessage
{
    [MaxLength(50)] public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; } = DateTime.Now;
    public string ChannelId { get; set; }
}