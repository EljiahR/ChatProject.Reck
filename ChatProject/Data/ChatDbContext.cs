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
    
    public DbSet<ChatChannel> Channels { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ChatChannel>()
            .HasMany(e => e.Messages)
            .WithOne(e => e.Channel)
            .HasForeignKey(e => e.ChannelId)
            .IsRequired();
    }
}