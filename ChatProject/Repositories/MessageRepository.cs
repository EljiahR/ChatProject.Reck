using ChatProject.Data;
using ChatProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly DbContext _context;
    private readonly DbSet<ChatMessage> _dbSet;

    public MessageRepository(ChatDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<ChatMessage>();
    }

    public async Task AddMessageAsync(ChatMessage chatMessage)
    {
        await _context.Entry(chatMessage).Reference(m => m.Channel).LoadAsync();
        await _dbSet.AddAsync(chatMessage);
        await SaveChangesAsync();
    }

    public async Task<IEnumerable<ChatMessage>> GetAllMessagesAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        _context.SaveChangesAsync();
    }
}