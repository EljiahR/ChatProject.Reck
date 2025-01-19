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

    public async Task<IEnumerable<Message>> GetAllMessagesAsync()
    {
        return await _repository.GetAllMessagesAsync();
    }

    public async Task AddMessageAsync(Message message)
    {
        await _repository.AddMessageAsync(message);
    }
}