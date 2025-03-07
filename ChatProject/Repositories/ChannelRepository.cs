using ChatProject.Data;
using ChatProject.Helpers;
using ChatProject.Models;
using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Repositories;

public class ChannelRepository : IChannelRepository
{
    private readonly DbContext _context;
    private readonly DbSet<ChatChannel> _dbSet;
    private readonly DbSet<ChatUser> _users;

    public ChannelRepository(ChatDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<ChatChannel>();
        _users = _context.Set<ChatUser>();
    }

    public async Task<ChatChannel?> GetChannelByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<List<ChatChannelDto>> GetAllChannelsAsync()
    {
        return await _dbSet
            .Include(c => c.ChannelMessages)
            .Include(c => c.CreatedBy)
            .Include(c => c.Admins)
            .Include(c => c.Members)
            .AsNoTracking()
            .Select(c => ModelConverter.ChannelBoToDto(c))
            .ToListAsync();
    }

    public async Task<List<int>> GetAllUserChannelIdsAsync(string userId)
    {
        var user = await _users
            .Include(u => u.AdministeredChannels)
            .Include(u => u.MemberChannels)
            .Include(u => u.CreatedChannels)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return new List<int>();

        return user.MemberChannels
            .Concat(user.AdministeredChannels)
            .Concat(user.CreatedChannels)
            .Distinct()
            .Select(c => c.Id)
            .ToList();
    }


    public async Task<List<ChatChannelDto>> GetAllUserChannelsAsync(string userId)
    {
        var user = await _users
            .Include(u => u.AdministeredChannels)
                .ThenInclude(c => c.ChannelMessages)
            .Include(u => u.MemberChannels)
                .ThenInclude(c => c.ChannelMessages)
            .Include(u => u.CreatedChannels)
                .ThenInclude(c => c.ChannelMessages)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return new List<ChatChannelDto>();

        return user.MemberChannels
            .Concat(user.AdministeredChannels)
            .Concat(user.CreatedChannels)
            .Distinct()
            .Select(ModelConverter.ChannelBoToDto)
            .ToList();
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

    public async Task AddMemberToChannelAsync(int channelId, string userId)
    {
        var channel = await _dbSet.Include(c => c.Members).FirstOrDefaultAsync(x => x.Id == channelId);
        if (channel != null)
        {
            var user = await _users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                channel.Members.Add(user);
                await _context.SaveChangesAsync();
            }
            
        }
    }
    
    public async Task AddAdminToChannelAsync(int channelId, string userId)
    {
        var channel = await _dbSet.Include(c => c.Members).FirstOrDefaultAsync(x => x.Id == channelId);
        if (channel != null)
        {
            var user = await _users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                channel.Admins.Add(user);
                await _context.SaveChangesAsync();
            }
            
        }
    }
}