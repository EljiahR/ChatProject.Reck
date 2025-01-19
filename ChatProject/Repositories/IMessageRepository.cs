using ChatProject.Models;

namespace ChatProject.Repositories;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetAllMessagesAsync();
    Task AddMessageAsync(Message message);
}