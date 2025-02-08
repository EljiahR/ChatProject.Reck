using ChatProject.Models;
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
        builder.Entity<ChatChannel>()
            .HasMany(e => e.ChannelMessages)
            .WithOne()
            .HasForeignKey(e => e.ChannelId)
            .IsRequired();
        
        base.OnModelCreating(builder);
    }
}