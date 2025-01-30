using ChatProject.Data;
using ChatProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Repositories;

public class ChannelRepository : IChannelRepository
{
    private readonly DbContext _context;
    private readonly DbSet<ChatChannel> _dbSet;

    public ChannelRepository(ChatDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<ChatChannel>();
    }

    public async Task<IEnumerable<ChatChannel>> GetAllChannelsAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<ChatChannel>> GetAllUserChannelsAsync(ChatUser user)
    {
        return await _dbSet.Where(channel => channel.Members.Contains(user)).ToListAsync();
    }
}