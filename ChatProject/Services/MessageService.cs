using ChatProject.Models;
using ChatProject.Repositories;

namespace ChatProject.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _repository;

    public MessageService(IMessageRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Message> GetAllMessages()
    {
        return _repository.GetAllMessages();
    }

    public void AddMessage(Message message)
    {
        _repository.AddMessage(message);
    }
}