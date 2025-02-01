namespace ChatProject.Models;

public class ChatChannel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<string> AdminIds { get; set; } = new();
    public List<string> MemberIds { get; set; } = new();
    public List<Message> Messages { get; set; } = new();
}