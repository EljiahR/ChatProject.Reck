using ChatProject.Models;
using ChatProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }
    
    [HttpPost]
    public IActionResult RegisterUser(ChatUser user)
    {
        try
        {
            _service.RegisterUser(user);
            return Ok(user);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            return BadRequest("Encountered problem adding user.");
        }
    }
}