namespace ChatProject.Models;

public class Channel
{
    public int Id { get; set; }
    public List<ChatUser> Admins { get; set; } = new();
    public List<ChatUser> Members { get; set; } = new();
    public List<Message> Messages { get; set; } = new();
}