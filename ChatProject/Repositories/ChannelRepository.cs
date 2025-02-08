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

    public async Task<ChatChannel?> GetChannelByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<IEnumerable<ChatChannel>> GetAllChannelsAsync()
    {
        return await _dbSet.Include(c => c.ChannelMessages).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<ChatChannel>> GetAllUserChannelsAsync(string userId)
    {
        return await _dbSet.Where(channel => channel.CreatedBy == userId || channel.AdminIds.Contains(userId) || channel.MemberIds.Contains(userId)).ToListAsync();
    }

    public async Task<int> AddChannelAsync(ChatChannel newChannel)
    {
        await _dbSet.AddAsync(newChannel);
        await _context.SaveChangesAsync();
        return newChannel.Id;
    }

    public async Task AddMessageToChannelAsync(int id, ChatMessage chatMessage)
    {
        var channel = await _dbSet
            .Include(c => c.ChannelMessages)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (channel != null)
        {
            channel.ChannelMessages.Add(chatMessage);
            await _context.SaveChangesAsync();
        }
    }
}