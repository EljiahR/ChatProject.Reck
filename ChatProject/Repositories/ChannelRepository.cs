using ChatProject.Data;
using ChatProject.Helpers;
using ChatProject.Models;
using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;
using ChatProject.Models.JoinModels;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Repositories;

public class ChannelRepository : IChannelRepository
{
    private readonly DbContext _context;
    private readonly DbSet<ChatChannel> _dbSet;
    private readonly DbSet<ChatUser> _users;
    private readonly DbSet<ChannelUser> _channelUsers;

    public ChannelRepository(ChatDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<ChatChannel>();
        _users = _context.Set<ChatUser>();
        _channelUsers = _context.Set<ChannelUser>();
    }

    public async Task<ChatChannel?> GetChannelByIdAsync(string id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<List<ChatChannelDto>> GetAllChannelsAsync()
    {
        return await _dbSet
            .Include(c => c.ChannelMessages)
            .Include(c => c.ChannelUsers)
            .ThenInclude(cu => cu.User)
            .AsNoTracking()
            .Select(c => ModelConverter.MapChannelToDto(c))
            .ToListAsync();
    }

    public async Task<List<string>> GetAllUserChannelIdsAsync(string userId)
    {
        var user = await _users
            .Include(u => u.ChannelUsers)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        return user != null ? user.ChannelUsers.Select(cu => cu.ChannelId).ToList() : [];
        
    }


    public async Task<List<ChatChannelDto>> GetAllUserChannelsAsync(string userId)
    {
        var channels = await _dbSet
            .Where(c => c.ChannelUsers.Any(cu => cu.UserId == userId))
            .Include(c => c.ChannelUsers)
            .ThenInclude(cu => cu.User)
            .Include(c => c.ChannelMessages)
            .ToListAsync();

        return channels.Select(ModelConverter.MapChannelToDto).ToList();
    }

    public async Task<ChatChannelDto> AddChannelAsync(string userId, string channelName)
    {
        var newChannel = new ChatChannel
        {
            Id = Guid.NewGuid().ToString(),
            Name = channelName,
            CreatedById = userId,
            ChannelUsers = [new ChannelUser
            {
                UserId = userId,
                Role = ChannelRole.Creator
            }]
        };

        await _dbSet.AddAsync(newChannel);
        await _context.SaveChangesAsync();

        var createdChannel = await _dbSet
            .Include(c => c.ChannelUsers)
            .SingleOrDefaultAsync(c => c.Id == newChannel.Id);
        
        return ModelConverter.MapChannelToDto(createdChannel!);
    }

    public async Task AddMessageToChannelAsync(string id, ChatMessage chatMessage)
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

    public async Task AddMemberToChannelAsync(string channelId, string userId)
    {
        var entry = await _channelUsers.Where(cu => cu.ChannelId == channelId && cu.UserId == userId).FirstOrDefaultAsync();
        if (entry == null)
        {
            var newEntry = new ChannelUser
            {
                ChannelId = channelId,
                UserId = userId,
                Role = ChannelRole.Member
            };

            _channelUsers.Add(newEntry);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task AddAdminToChannelAsync(string channelId, string userId)
    {
        var entry = await _channelUsers.Where(cu => cu.ChannelId == channelId && cu.UserId == userId).FirstOrDefaultAsync();
        if (entry == null || entry.Role != ChannelRole.Admin)
        {
            var newEntry = new ChannelUser
            {
                ChannelId = channelId,
                UserId = userId,
                Role = ChannelRole.Admin
            };

            _channelUsers.Add(newEntry);
            await _context.SaveChangesAsync();
        }
    }
}