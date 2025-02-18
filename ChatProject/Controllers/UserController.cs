using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatProject.Helpers;
using ChatProject.Models;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChatProject.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly SignInManager<ChatUser> _signInManager;
    private readonly UserManager<ChatUser> _userManager;
    private readonly IChannelService _channelService;
    private readonly IConfiguration _configuration;

    public UserController(SignInManager<ChatUser> signInManager, UserManager<ChatUser> userManager, IChannelService channelService, IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _channelService = channelService;
        _configuration = configuration;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterDto model)
    {
        if (ModelState.IsValid)
        {
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

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                return Ok(new { message = "User created successfully!" });
            }

            return BadRequest(result.Errors);
        }

        return BadRequest("Invalid data type.");
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
    [Route("UserInfo")]
    public async Task<IActionResult> GetUserInfo()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new { user.UserName });
    }

    [HttpGet]
    [Route("Status")]
    public async Task<IActionResult> LoginStatus()
    {
        if (!User.Identity!.IsAuthenticated) return Unauthorized();
        
        var userBo = await _userManager.GetUserAsync(User);
        var channelBos = await _channelService.GetAllUserChannelsAsync(userBo!.Id);
        var friends = await _userManager.Users.Where(user => userBo.FriendIds.Contains(user.Id)).ToListAsync();
        
        return Ok(ModelConverter.UserBoToDto(userBo!, channelBos, friends));
        
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
            .Select(user => ModelConverter.ChatUserToPersonDto(user))
            .ToListAsync();
        
        if (people.Count > 0)
        {
            return Ok(people);
        }

        return NotFound();
    }

    [HttpPost]
    [Route("AddFriend")]
    public async Task<IActionResult> AddFriend(string friendId)
    {
        var newFriend = await _userManager.FindByIdAsync(friendId);
        if (newFriend == null)
        {
            return NotFound($"User with id ({friendId}) was not found.");
        }
        
        var user = await _userManager.GetUserAsync(User);
        user!.FriendIds.Add(friendId);
        await _userManager.UpdateAsync(user);

        return Ok(ModelConverter.ChatUserToPersonDto(newFriend));
    }
}