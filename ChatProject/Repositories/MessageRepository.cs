using ChatProject.Data;
using ChatProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ChatDbContext _context;
    private readonly DbSet<ChatMessage> _dbSet;

    public MessageRepository(ChatDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<ChatMessage>();
    }

    public async Task AddMessageAsync(ChatMessage chatMessage)
    {
        await _dbSet.AddAsync(chatMessage);

        var channel = await _context.ChatChannels.FindAsync(chatMessage.ChannelId);
        if (channel != null)
        {
            channel.ChannelMessages.Add(chatMessage);
        }
        
        await SaveChangesAsync();
    }

    public async Task<IEnumerable<ChatMessage>> GetAllMessagesAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<ChatMessage?> GetMessageByIdAsync(string id)
    {
        return await _dbSet.FindAsync(id);
    }

    private async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}