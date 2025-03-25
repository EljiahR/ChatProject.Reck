using ChatProject.Models;

namespace ChatProject.Repositories;

public interface IMessageRepository
{
    Task<IEnumerable<ChatMessage>> GetAllMessagesAsync();
    Task<ChatMessage?> GetMessageByIdAsync(string id);
    Task AddMessageAsync(ChatMessage chatMessage);
}