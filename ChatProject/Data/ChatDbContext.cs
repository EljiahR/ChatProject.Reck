using ChatProject.Models;
using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;
using ChatProject.Models.JoinModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Data;

public class ChatDbContext : IdentityDbContext<ChatUser>
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<ChatChannel> ChatChannels { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Join table for User < - > Channel
        builder.Entity<ChannelUser>()
            .HasKey(cu => new { cu.UserId, cu.ChannelId });

        // One-to-many: ChannelUser (join table) to User
        builder.Entity<ChannelUser>()
            .HasOne(cu => cu.User)
            .WithMany(u => u.ChannelUsers)
            .HasForeignKey(u => u.UserId);

        // One-to-many: ChannelUser (join table) to Channel
        builder.Entity<ChannelUser>()
            .HasOne(cu => cu.Channel)
            .WithMany(c => c.ChannelUsers)
            .HasForeignKey(cu => cu.ChannelId);
        
        // Many-to-one: ChatMessage
        builder.Entity<ChatChannel>()
            .HasMany(e => e.ChannelMessages)
            .WithOne()
            .HasForeignKey(e => e.ChannelId)
            .IsRequired();

        // Join table for User < - > User
        builder.Entity<Friendship>()
            .HasKey(f => new { f.InitiatorId, f.ReceiverId });
        
        builder.Entity<Friendship>()
            .HasOne(f => f.Initiator)
            .WithMany(u => u.FriendsInitiated)
            .HasForeignKey(f => f.InitiatorId);
        
        builder.Entity<Friendship>()
            .HasOne(f => f.Receiver)
            .WithMany(u => u.FriendsReceived)
            .HasForeignKey(f => f.ReceiverId);
        
        base.OnModelCreating(builder);
    }
}