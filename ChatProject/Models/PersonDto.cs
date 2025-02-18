namespace ChatProject.Models;

public class PersonDto
{
    public string? UserName { get; set; }
    public string? UserId { get; set; }
    public bool IsFriend { get; set; } = false;
}