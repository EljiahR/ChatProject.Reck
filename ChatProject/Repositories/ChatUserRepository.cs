using ChatProject.Data;
using ChatProject.Helpers;
using ChatProject.Models.ChatUserModels;
using ChatProject.Models.JoinModels;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Repositories;

public class ChatUserRepository : IChatUserRepository
{
    private readonly DbContext _context;
    private readonly DbSet<ChatUser> _dbSet;
    private readonly DbSet<Friendship> _friendships;

    public ChatUserRepository(ChatDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<ChatUser>();
        _friendships = _context.Set<Friendship>();
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
            .Include(u => u.FriendsInitiated)
                .ThenInclude(f => f.Receiver)
            .Include(u => u.FriendsReceived)
                .ThenInclude(f => f.Initiator)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user != null ? ModelConverter.MapChatUserToDto(user) : null;
    }

    public async Task AddFriends(string initiatorId, string receiverId)
    {
        var friendship = await _friendships.Where(f => f.InitiatorId == initiatorId && f.ReceiverId == receiverId)
            .FirstOrDefaultAsync();
        if (friendship == null)
        {
            var newFriendship = new Friendship
            {
                InitiatorId = initiatorId,
                ReceiverId = receiverId
            };

            await _friendships.AddAsync(newFriendship);
            await _context.SaveChangesAsync();
        }
    }

}