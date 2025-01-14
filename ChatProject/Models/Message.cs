namespace ChatProject.Models;

public class Message(string user, string content)
{
    public string Username = user;
    public string Content = content;
    public DateTime SentAt = DateTime.Now;
}