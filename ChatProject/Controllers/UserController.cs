using System.Security.Claims;
using ChatProject.ConfigModels;
using ChatProject.Helpers;
using ChatProject.Models;
using ChatProject.Models.ChatUserModels;
using ChatProject.Models.FromBodyModels;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ChatProject.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly SignInManager<ChatUser> _signInManager;
    private readonly UserManager<ChatUser> _userManager;
    private readonly IChatUserService _userService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly JwtSettings _jwtSettings;

    public UserController(SignInManager<ChatUser> signInManager, UserManager<ChatUser> userManager, IChatUserService userService, IRefreshTokenService refreshTokenService, JwtSettings jwtSettings)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userService = userService;
        _refreshTokenService = refreshTokenService;
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
        
        var passwordMatches = await _userManager.CheckPasswordAsync(user, model.Password!);
        if (passwordMatches)
        {
            var userDto = await _userService.GetUserDtoAsync(user.Id);
            var accessToken = TokenGenerators.GenerateAccessToken(user.UserName!, user.Id, _jwtSettings);
            var refreshToken = new RefreshToken
            {
                Token = TokenGenerators.GenerateRefreshToken(),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };
            await _refreshTokenService.AddTokenAsync(refreshToken);
            return Ok(new { message = "User created successfully!", info = userDto, accessToken, refreshToken = refreshToken.Token });
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
                var accessToken = TokenGenerators.GenerateAccessToken(user.UserName!, user.Id, _jwtSettings);
                var refreshToken = new RefreshToken
                {
                    Token = TokenGenerators.GenerateRefreshToken(),
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                };
                await _refreshTokenService.AddTokenAsync(refreshToken);

                return Ok(new { message = "Login successful!", info = userDto, accessToken, refreshToken = refreshToken.Token });
            }
            return Unauthorized(new { message = "Invalid username or password." });
        }

        return BadRequest("Invalid data.");
    }

    [HttpPost]
    [Route("Refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenBody model)
    {
        if (!ModelState.IsValid) {
            return BadRequest();
        }

        var existingToken = await _refreshTokenService.GetRefreshTokenAsync(model.RefreshToken);
        if (existingToken is { IsRevoked: false } && existingToken.UserId == model.UserId) 
        {
            var user = await _userService.GetUserDtoAsync(model.UserId);
            var accessToken = TokenGenerators.GenerateAccessToken(user!.UserName!, user.Id, _jwtSettings);
            return Ok(new { accessToken });
        }

        return Unauthorized();
    }

    [HttpGet]
    [Route("Status")]
    public async Task<IActionResult> LoginStatus()
    {
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
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _refreshTokenService.DeleteUserTokensAsync(currentUserId);

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