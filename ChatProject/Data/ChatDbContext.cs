using ChatProject.Models;
using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;
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
        // Many-to-one: ChatMessage
        builder.Entity<ChatChannel>()
            .HasMany(e => e.ChannelMessages)
            .WithOne()
            .HasForeignKey(e => e.ChannelId)
            .IsRequired();

        // One-to-many: CreatedBy
        builder.Entity<ChatChannel>()
            .HasOne(c => c.CreatedBy)
            .WithMany(u => u.CreatedChannels)
            .HasForeignKey(c => c.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Many-to-many: Admins
        builder.Entity<ChatChannel>()
            .HasMany(c => c.Admins)
            .WithMany(u => u.AdministeredChannels)
            .UsingEntity<Dictionary<string, object>>(
                "ChannelAdmins",
                j => j.HasOne<ChatUser>().WithMany().HasForeignKey("UserId"),
                j => j.HasOne<ChatChannel>().WithMany().HasForeignKey("ChannelId"));
        
        // Many-to-many: Members
        builder.Entity<ChatChannel>()
            .HasMany(c => c.Members)
            .WithMany(u => u.MemberChannels)
            .UsingEntity<Dictionary<string, object>>(
                "ChannelMembers",
                j => j.HasOne<ChatUser>().WithMany().HasForeignKey("UserId"),
                j => j.HasOne<ChatChannel>().WithMany().HasForeignKey("ChannelId"));
        
        base.OnModelCreating(builder);
    }
}