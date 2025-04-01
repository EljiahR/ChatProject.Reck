namespace ChatProject.Models.ChatUserModels;

public class PersonDto
{
    public string? UserName { get; set; }
    public string? Id { get; set; }
    public bool IsFriend { get; set; } = false;
}