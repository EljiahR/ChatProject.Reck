using ChatProject.Models;

namespace ChatProject.Repositories;

public interface IMessageRepository
{
    Task<IEnumerable<ChatMessage>> GetAllMessagesAsync();
    Task AddMessageAsync(ChatMessage chatMessage);
}