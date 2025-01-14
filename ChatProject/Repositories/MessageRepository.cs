using ChatProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly DbContext _context;
    private readonly DbSet<Message> _dbSet;

    public MessageRepository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<Message>();
    }

    public void AddMessage(Message message)
    {
        _dbSet.Add(message);
        SaveChanges();
    }

    public IEnumerable<Message> GetAllMessages()
    {
        return _dbSet.ToList();
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}