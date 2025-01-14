using ChatProject.Models;

namespace ChatProject.Services;

public interface IUserService
{
    void RegisterUser(ChatUser user);
    IEnumerable<ChatUser> GetAllUsers();
}