namespace ChatProject.Models;

public class ChatChannel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<ChatUser> Admins { get; set; } = new();
    public List<ChatUser> Members { get; set; } = new();
    public List<Message> Messages { get; set; } = new();
}