namespace ChatProject.Models.FromBodyModels;

public class RefreshTokenBody 
{
    public string UserId { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}