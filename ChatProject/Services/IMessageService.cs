using ChatProject.Models;

namespace ChatProject.Services;

public interface IMessageService
{
    Task<IEnumerable<ChatMessage>> GetAllMessagesAsync();
    Task AddMessageAsync(ChatMessage chatMessage);
    Task<ChatMessage?> GetMessageByIdAsync(string id);

}