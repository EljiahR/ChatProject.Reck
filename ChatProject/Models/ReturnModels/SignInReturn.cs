using ChatProject.Models.ChatUserModels;

namespace ChatProject.Models.ReturnModels;

public class SignInReturn
{
    public string Message { get; set; } = string.Empty;
    public ChatUserDto? Info { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}