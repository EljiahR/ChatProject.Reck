using ChatProject.Models;

namespace ChatProject.Services;

public interface IMessageService
{
    IEnumerable<Message> GetAllMessages();
    void AddMessage(Message message);
}