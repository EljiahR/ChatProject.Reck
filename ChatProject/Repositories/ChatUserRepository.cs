using ChatProject.Data;
using ChatProject.Models.ChatUserModels;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Repositories;

public class ChatUserRepository : IChatUserRepository
{
    private readonly DbContext _context;
    private readonly DbSet<ChatUser> _dbSet;

    public ChatUserRepository(ChatDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<ChatUser>();
    }
    
    public async Task<ChatUser?> GetUserWithChannelsByIdAsync(string userId)
    {
        return await _dbSet
            .Include(u => u.ChannelUsers)
                .ThenInclude(cu => cu.Channel)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<ChatUserDto?> GetUserDtoAsync(string userId)
    {
        var user = await _dbSet
            .Include(u => u.ChannelUsers)
                .ThenInclude(cu => cu.Channel)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}