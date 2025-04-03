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
    private readonly DbSet<ChatChannel> _channels;
    private readonly DbSet<ChatUser> _users;
    private readonly DbSet<ChannelUser> _channelUsers;

    public ChannelRepository(ChatDbContext context)
    {
        _context = context;
        _channels = _context.Set<ChatChannel>();
        _users = _context.Set<ChatUser>();
        _channelUsers = _context.Set<ChannelUser>();
    }

    public async Task<ChatChannel?> GetChannelByIdAsync(string id)
    {
        return await _channels.FindAsync(id);
    }
    public async Task<List<ChatChannelDto>> GetAllChannelsAsync()
    {
        return await _channels
            .Include(c => c.ChannelMessages)
            .Include(c => c.ChannelUsers)
            .ThenInclude(cu => cu.User)
            .AsNoTracking()
            .Select(c => ModelConverter.MapChannelToDto(c, UserStatus.Active))
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
        var channels = await _channels
            .Where(c => c.ChannelUsers.Any(cu => cu.UserId == userId))
            .Include(c => c.ChannelUsers.Where(cu => cu.Status != UserStatus.Banned))
                .ThenInclude(cu => cu.User)
            .Include(c => c.ChannelMessages)
            .ToListAsync();

        return channels.Select(c => ModelConverter.MapChannelToDto(c, c.ChannelUsers.First(cu => cu.UserId == userId).Status)).ToList();
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
                Role = ChannelRole.Creator,
                Status = UserStatus.Active
            }]
        };

        await _channels.AddAsync(newChannel);
        await _context.SaveChangesAsync();

        var createdChannel = await _channels
            .Include(c => c.ChannelUsers)
            .SingleOrDefaultAsync(c => c.Id == newChannel.Id);
        
        return ModelConverter.MapChannelToDto(createdChannel!, UserStatus.Active);
    }

    public async Task AddMessageToChannelAsync(string id, ChatMessage chatMessage)
    {
        var channel = await _channels
            .Include(c => c.ChannelMessages)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (channel != null)
        {
            channel.ChannelMessages.Add(chatMessage);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ChannelUserDto> InviteMemberToChannelAsync(string channelId, string userId)
    {
        var entry = await _channelUsers.Where(cu => cu.ChannelId == channelId && cu.UserId == userId).FirstOrDefaultAsync();
        if (entry == null)
        {
            var newEntry = new ChannelUser
            {
                ChannelId = channelId,
                UserId = userId,
                Role = ChannelRole.Member,
                Status = UserStatus.Pending
            };

            _channelUsers.Add(newEntry);
            await _context.SaveChangesAsync();
            return ModelConverter.MapChannelUserToDto(newEntry);
        }

        return ModelConverter.MapChannelUserToDto(entry);
    }
    
    public async Task<ChannelUserDto> InviteAdminToChannelAsync(string channelId, string userId)
    {
        var entry = await _channelUsers.Where(cu => cu.ChannelId == channelId && cu.UserId == userId).FirstOrDefaultAsync();
        if (entry == null || entry.Role != ChannelRole.Admin)
        {
            var newEntry = new ChannelUser
            {
                ChannelId = channelId,
                UserId = userId,
                Role = ChannelRole.Admin,
                Status = UserStatus.Pending
            };

            _channelUsers.Add(newEntry);
            await _context.SaveChangesAsync();
            return ModelConverter.MapChannelUserToDto(newEntry);
        }

        return ModelConverter.MapChannelUserToDto(entry);
    }

    public async Task<ChatChannelDto> ConfirmChannelInviteAsync(string channelId, string userId)
    {
        var entry = await _channelUsers.Where(cu => cu.ChannelId == channelId && cu.UserId == userId).FirstOrDefaultAsync();
        if (entry == null)
        {
            throw new InvalidOperationException("Error finding entry");
        }
        
        entry.Status = UserStatus.Active;
        await _context.SaveChangesAsync();
        var channel = await _channels
            .Include(c => c.ChannelUsers.Where(cu => cu.Status != UserStatus.Banned))
                .ThenInclude(cu => cu.User)
            .Include(c => c.ChannelMessages)
            .FirstOrDefaultAsync(c => c.Id == channelId);

        if (channel == null)
        {
            throw new InvalidOperationException("Error finding channel");
        }
        
        return ModelConverter.MapChannelToDto(channel, UserStatus.Active);
        
    }


    public async Task RemoveUserFromChannelAsync(string channelId, string userId)
    {
        var entry = await _channelUsers.Where(cu => cu.ChannelId == channelId && cu.UserId == userId).FirstOrDefaultAsync();
        if (entry != null)
        {
            _channelUsers.Remove(entry);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveMessageFromChannelAsync(string channelId, string messageId)
    {
        var channel = await _channels
            .Include(c => c.ChannelMessages)
            .Where(c => c.Id == channelId)
            .FirstOrDefaultAsync();
        if (channel != null)
        {
            var messageToDelete = channel.ChannelMessages.FirstOrDefault(m => m.Id == messageId);
            if (messageToDelete != null)
            {
                channel.ChannelMessages.Remove(messageToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}