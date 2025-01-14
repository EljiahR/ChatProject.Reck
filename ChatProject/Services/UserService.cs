using ChatProject.Models;
using ChatProject.Repositories;

namespace ChatProject.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public void RegisterUser(ChatUser user)
    {
        _repository.RegisterUser(user);
    }

    public IEnumerable<ChatUser> GetAllUsers()
    {
        return _repository.GetAllUsers();
    }
}