using ChatProject.Models;

namespace ChatProject.Services;

public interface IMessageService
{
    Task<IEnumerable<Message>> GetAllMessagesAsync();
    Task AddMessageAsync(Message message);
}