using ChatProject.Models;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatProject.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly SignInManager<ChatUser> _signInManager;
    private readonly UserManager<ChatUser> _userManager;
    public UserController(SignInManager<ChatUser> signInManager, UserManager<ChatUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
    

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return Ok("Logged out successfully!");
    }
}