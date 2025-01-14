using ChatProject.Models;

namespace ChatProject.Repositories;

public interface IMessageRepository
{
    IEnumerable<Message> GetAllMessages();
    void AddMessage(Message message);
}