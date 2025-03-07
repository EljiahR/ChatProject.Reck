using System.Security.Claims;
using ChatProject.Helpers;
using ChatProject.Hubs;
using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatProject.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class ChannelController : ControllerBase
{
    private readonly IChannelService _channelService;
    private readonly IChatUserService _userService;
    private readonly UserManager<ChatUser> _userManager;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ConnectionManager _connectionManager;

    public ChannelController(IChannelService channelService, IChatUserService userService, UserManager<ChatUser> userManager, IHubContext<ChatHub> hubContext, ConnectionManager connectionManager)
    {
        _channelService = channelService;
        _userService = userService;
        _userManager = userManager;
        _hubContext = hubContext;
        _connectionManager = connectionManager;
    }

    [HttpPost]
    [Route("New")]
    public async Task<IActionResult> CreateChannel([FromBody] NewChannelDto model)
    {
        var user = await _userManager.GetUserAsync(User);
        var newChannel = new ChatChannel { Name = model.Name, CreatedById = user!.Id};
        
        var newId = await _channelService.AddChannelAsync(newChannel);
        await _userManager.UpdateAsync(user);
        
        // Updating connection manager so user can use the new channel
        _connectionManager.AddChannel(user.Id, newId);

        var connectionIds = _connectionManager.GetConnections(user.Id);
        foreach (var connectionId in connectionIds)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, newChannel.Id.ToString());
        }
        
        return Ok(ModelConverter.ChannelBoToDto(newChannel));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllChannels()
    {
        return Ok(await _channelService.GetAllChannelsAsync());
    }

    [HttpPost]
    [Route("{channelId}/add/{userId}")]
    public async Task<IActionResult> AddUserToChannel(int channelId, string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userService.GetUserWithChannelsByIdAsync(currentUserId);
        if (user == null)
        {
            return Unauthorized("User was not found.");
        }
        
        if (!user.CreatedChannels.Concat(user.AdministeredChannels).Concat(user.MemberChannels).Any(c => c.CreatedBy == user || c.Admins.Contains(user) || c.Members.Contains(user)))
        {
            return Unauthorized("User not a member of the channel.");
        }

        var newChannelUser = await _userService.GetUserWithChannelsByIdAsync(userId);

        if (newChannelUser == null)
        {
            return NotFound($"User with id '{userId}' was not found.");
        } else if (newChannelUser.MemberChannels.Concat(newChannelUser.AdministeredChannels).Concat(newChannelUser.CreatedChannels).Any(c => c.Id == channelId))
        {
            return BadRequest("User already in channel.");
        }

        try
        {
            await _channelService.AddMemberToChannelAsync(channelId, userId);
            var connectionIds = _connectionManager.GetConnections(userId);
            foreach (var connectionId in connectionIds)
            {
                await _hubContext.Groups.AddToGroupAsync(connectionId, channelId.ToString());
            }

            return Ok("User added to channel successfully!");
        }
        catch (Exception error)
        {
            return BadRequest("Error occured when adding user: " + error.Message);
        }
    }
}