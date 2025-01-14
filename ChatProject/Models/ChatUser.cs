using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace ChatProject.Models;

public class ChatUser : IdentityUser
{
    public override string? UserName { get; set; }
    public override string? Email { get; set; }
}