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
    private readonly DbSet<ChannelUser> _channelUsers;
    private readonly IConfiguration _config;

    public ChatUserRepository(ChatDbContext context, IConfiguration config)
    {
        _context = context;
        _dbSet = _context.Set<ChatUser>();
        _friendships = _context.Set<Friendship>();
        _channelUsers = _context.Set<ChannelUser>();
        _config = config;
    }
    
    public async Task<ChatUser?> GetUserWithChannelsByIdAsync(string userId)
    {
        return await _dbSet
            .Include(u => u.ChannelUsers.Where(cu => cu.Status != UserStatus.Banned))
                .ThenInclude(cu => cu.Channel)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<ChatUserDto?> GetUserDtoAsync(string userId)
    {
        var user = await _dbSet
            .Include(u => u.ChannelUsers.Where(cu => cu.Status != UserStatus.Banned))
                .ThenInclude(cu => cu.Channel)
                    .ThenInclude(c => c.ChannelUsers)
                        .ThenInclude(cu => cu.User)
            .Include(u => u.ChannelUsers.Where(cu => cu.Status != UserStatus.Banned))
                .ThenInclude(cu => cu.Channel)
                    .ThenInclude(c => c.ChannelMessages.OrderByDescending(cm => cm.SentAt).Take(50))
            .Include(u => u.FriendsInitiated)
                .ThenInclude(f => f.Receiver)
            .Include(u => u.FriendsReceived)
                .ThenInclude(f => f.Initiator)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var homeId = _config["HomeId"];
        if (!string.IsNullOrWhiteSpace(homeId) && user != null && !user.ChannelUsers.Any(cu => cu.ChannelId == homeId))
        {
            var newEntry = new ChannelUser
            {
                UserId = user.Id,
                ChannelId = homeId,
                Role = ChannelRole.Member,
                Status = UserStatus.Active
            };
            _channelUsers.Add(newEntry);
            await _context.SaveChangesAsync();
            await _context.Entry(newEntry).Reference(cu => cu.User).LoadAsync();
            await _context.Entry(newEntry).Reference(cu => cu.Channel).LoadAsync();
        }

        return user != null ? ModelConverter.MapChatUserToDto(user) : null;
    }

    public async Task<FriendshipDto?> ConfirmFriendAsync(string initiatorId, string receiverId)
    {
        var friendship = await _friendships.Where(f => f.InitiatorId == initiatorId && f.ReceiverId == receiverId)
            .Include(f => f.Initiator)
            .Include(f => f.Receiver)
            .FirstOrDefaultAsync();
        
        if (friendship != null)
        {
            friendship.Status = FriendshipStatus.Friends;
            await _context.SaveChangesAsync();
            return ModelConverter.MapFriendshipToDto(friendship);
        }

        return null;
    }

    public async Task<FriendshipDto?> RequestFriendAsync(string initiatorId, string receiverId)
    {
        var friendship = await _friendships.Where(f => f.InitiatorId == initiatorId && f.ReceiverId == receiverId)
            .FirstOrDefaultAsync();
        if (friendship == null)
        {
            var newFriendship = new Friendship
            {
                InitiatorId = initiatorId,
                ReceiverId = receiverId,
                Status = FriendshipStatus.Pending
            };

            await _friendships.AddAsync(newFriendship);
            await _context.SaveChangesAsync();

            await _context.Entry(newFriendship).Reference(f => f.Initiator).LoadAsync();
            await _context.Entry(newFriendship).Reference(f => f.Receiver).LoadAsync();

            
            return ModelConverter.MapFriendshipToDto(newFriendship!);
        }

        return null;
    }

}