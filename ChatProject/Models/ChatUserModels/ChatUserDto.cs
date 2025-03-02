using ChatProject.Models.ChatChannelModels;

namespace ChatProject.Models.ChatUserModels;

public class ChatUserDto
{
    
    public required string UserName { get; set; }
    public required List<ChatChannelDto> Channels { get; set; }
    public required List<PersonDto> Friends { get; set; }
}