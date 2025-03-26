using ChatProject.Helpers;
using ChatProject.Models.ChatUserModels;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly SignInManager<ChatUser> _signInManager;
    private readonly UserManager<ChatUser> _userManager;
    private readonly IChatUserService _userService;

    public UserController(SignInManager<ChatUser> signInManager, UserManager<ChatUser> userManager, IChatUserService userService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userService = userService;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid data type.");
        }
        
        var existingUser = await _userManager.FindByNameAsync(model.UserName);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Name taken"});
        }
        
        var user = new ChatUser
        {
            UserName = model.UserName,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (signInResult.Succeeded)
        {
            return Ok(new { message = "User created successfully!" });
        }
        return BadRequest("Problem signing in user.");
        

    }

    [HttpPost("SignIn")]
    [AllowAnonymous]
    public async Task<IActionResult> SignInUser([FromBody] LoginDto model)
    {
        await _signInManager.SignOutAsync();
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.UserName!);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password!, false, false);
            if (result.Succeeded)
            {
                return Ok(new { message = "Login successful!" });
            }
            return Unauthorized(new { message = "Invalid username or password." });
        }

        return BadRequest("Invalid data.");
    }

    [HttpGet]
    [Route("Status")]
    public async Task<IActionResult> LoginStatus()
    {
        if (!User.Identity!.IsAuthenticated) return Unauthorized();

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized("Error finding user");
        }
        
        return Ok(ModelConverter.MapChatUserToDto(user));
        
    }

    [HttpPost]
    [Route("SignOut")]
    public async Task<IActionResult> SignOutUser()
    {
        await _signInManager.SignOutAsync();

        return Ok("Logged out successfully!");
    }

    [HttpPost]
    [Route("ChangeName")]
    public async Task<IActionResult> ChangeUsername(string newName)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            await _userManager.SetUserNameAsync(user, newName);

            return Ok("Username successfully changed to " + newName);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("FindByName/{searchQuery}")]
    public async Task<IActionResult> FindUsersByName(string searchQuery)
    {
        var client = await _userManager.GetUserAsync(User);
        
        var people = await _userManager.Users.Where(user => user.UserName != client!.UserName && user.UserName!.ToLower().Contains(searchQuery.ToLower()))
            .Select(user => ModelConverter.MapChatUserToPersonDto(user))
            .ToListAsync();
        
        if (people.Count > 0)
        {
            return Ok(people);
        }

        return NotFound();
    }

    [HttpPost]
    [Route("AddFriend")]
    public async Task<IActionResult> AddFriend([FromBody]NewFriendDto model)
    {
        var newFriend = await _userManager.FindByIdAsync(model.Id);
        if (newFriend == null)
        {
            return NotFound($"User with id ({model.Id}) was not found.");
        }
        
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"User was not found.");
        }

        await _userService.AddFriends(user.Id, newFriend.Id);

        return Ok(ModelConverter.MapChatUserToPersonDto(newFriend));
    }
}