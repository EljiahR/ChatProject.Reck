using System.Security.Claims;
using ChatProject.ConfigModels;
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
    private readonly JwtSettings _jwtSettings;

    public UserController(SignInManager<ChatUser> signInManager, UserManager<ChatUser> userManager, IChatUserService userService, JwtSettings jwtSettings)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userService = userService;
        _jwtSettings = jwtSettings;
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
            var userDto = await _userService.GetUserDtoAsync(user.Id);
            return Ok(new { message = "User created successfully!", info = userDto });
        }
        return BadRequest("Problem signing in user.");
        

    }

    [HttpPost("SignIn")]
    [AllowAnonymous]
    public async Task<IActionResult> SignInUser([FromBody] LoginDto model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.UserName!);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var passwordMatches = await _userManager.CheckPasswordAsync(user, model.Password!);
            if (passwordMatches)
            {
                var userDto = await _userService.GetUserDtoAsync(user.Id);
                var token = TokenGenerators.GenerateAccessToken(user.UserName!, _jwtSettings);
                return Ok(new { message = "Login successful!", info = userDto, AccessToken = token });
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

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userService.GetUserDtoAsync(currentUserId);
        if (user == null)
        {
            return Unauthorized("Error finding user");
        }
        
        return Ok(user);
        
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
            .Select(user => ModelConverter.MapChatUserToPersonDto(user, false))
            .ToListAsync();
        
        if (people.Count > 0)
        {
            return Ok(people);
        }

        return NotFound();
    }

    [HttpPost]
    [Route("RequestFriend")]
    public async Task<IActionResult> RequestFriend([FromBody]FriendRequestDto model)
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

        await _userService.RequestFriendAsync(user.Id, newFriend.Id);

        return Ok("Friend request sent!");
    }

    [HttpPost]
    [Route("ConfirmFriendRequest")]
    public async Task<IActionResult> ConfirmFriendRequest([FromBody] FriendRequestDto model)
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

        await _userService.ConfirmFriendAsync(newFriend.Id, user.Id);

        return Ok("Friend request sent!");
    }
}