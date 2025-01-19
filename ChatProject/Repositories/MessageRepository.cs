using ChatProject.Data;
using ChatProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly DbContext _context;
    private readonly DbSet<Message> _dbSet;

    public MessageRepository(ChatDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<Message>();
    }

    public async Task AddMessageAsync(Message message)
    {
        await _dbSet.AddAsync(message);
        SaveChanges();
    }

    public async Task<IEnumerable<Message>> GetAllMessagesAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}