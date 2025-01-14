using ChatProject.Data;
using ChatProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbContext _context;
    private readonly DbSet<ChatUser> _dbSet;

    public UserRepository(ChatDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<ChatUser>();
    }
    
    public void RegisterUser(ChatUser user)
    {
        _dbSet.Add(user);
        SaveChanges();
    }

    public IEnumerable<ChatUser> GetAllUsers()
    {
        return _dbSet.ToList();
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}