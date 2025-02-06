namespace ChatProject.Models;

public class RegisterDto
{
    public required string UserName { get; set; }
    public string? Email { get; set; }
    public required string Password { get; set; }
}