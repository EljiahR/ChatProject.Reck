namespace ChatProject.Models.ChatChannelModels;

public class UpdateChatChannel {
    public required string Id { get; set; }
    public string? Name { get; set; }
    public bool? IsFrozen { get; set; }
}