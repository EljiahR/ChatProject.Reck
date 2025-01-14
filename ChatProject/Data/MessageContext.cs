using ChatProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Data;

public class MessageContext : DbContext
{
    public MessageContext(DbContextOptions<MessageContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<Message> ChatLog { get; set; }
}