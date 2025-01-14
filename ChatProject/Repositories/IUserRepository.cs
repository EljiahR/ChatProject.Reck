using ChatProject.Models;

namespace ChatProject.Repositories;

public interface IUserRepository
{
    void RegisterUser(ChatUser user);
    IEnumerable<ChatUser> GetAllUsers();
}