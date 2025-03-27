using System.Security.Claims;
using ChatProject.Helpers;
using ChatProject.Hubs;
using ChatProject.Models.ChatChannelModels;
using ChatProject.Models.ChatUserModels;
using ChatProject.Models.FromBodyModels;
using ChatProject.Models.JoinModels;
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
        
        var newChannel = await _channelService.AddChannelAsync(user!.Id, model.Name);
        await _userManager.UpdateAsync(user);
        
        // Updating connection manager so user can use the new channel
        _connectionManager.AddChannel(user.Id, newChannel.Id);

        var connectionIds = _connectionManager.GetConnections(user.Id);
        foreach (var connectionId in connectionIds)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, newChannel.Id);
        }
        
        return Ok(newChannel);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllChannels()
    {
        return Ok(await _channelService.GetAllChannelsAsync());
    }

    [HttpPost]
    [Route("InviteUserToChannel")]
    public async Task<IActionResult> InviteUserToChannel([FromBody] ChannelUserDto model)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userService.GetUserWithChannelsByIdAsync(currentUserId);
        if (user == null)
        {
            return Unauthorized("User was not found.");
        }
        
        if (user.ChannelUsers.All(cu => cu.ChannelId != model.channelId))
        {
            return Unauthorized("User not a member of the channel.");
        }

        var newChannelUser = await _userService.GetUserWithChannelsByIdAsync(model.userId);

        if (newChannelUser == null)
        {
            return NotFound($"User with id '{model.userId}' was not found.");
        }
        
        if (newChannelUser.ChannelUsers.Any(cu => cu.ChannelId == model.channelId))
        {
            return BadRequest("User already in channel.");
        }

        try
        {
            if (model.role == ChannelRole.Admin)
            {
                await _channelService.InviteAdminToChannelAsync(model.channelId, model.userId);
            }
            else
            {
                await _channelService.InviteMemberToChannelAsync(model.channelId, model.userId);
            }
            
            // WILL BE CHANGED AND MOVED TO CONFIRMATION
            // var connectionIds = _connectionManager.GetConnections(model.userId);
            // foreach (var connectionId in connectionIds)
            // {
            //     await _hubContext.Groups.AddToGroupAsync(connectionId, model.channelId);
            // }

            return Ok(new { message = "User invite sent successfully!"});
        }
        catch (Exception error)
        {
            return BadRequest("Error occured when adding user: " + error.Message);
        }
    }

    [HttpPost]
    [Route("ConfirmChannelInvite")]
    public async Task<IActionResult> ConfirmChannelInviteAsync([FromBody] ChannelIdDto model)
    {
        return BadRequest("Not implemented yet");
    }

    [HttpPost]
    [Route("RemoveUserFromChannel")]
    public async Task<IActionResult> RemoveUserFromChannel([FromBody] ChannelUserDto model)
    {
        try
        {
            await _channelService.RemoveUserFromChannelAsync(model.channelId, model.userId);
            return Ok(new { message = "User successfully removed from channel" });
        }
        catch (Exception error)
        {
            return BadRequest("Error occured when trying to remove user from channel: " + error.Message);
        }
    }
    
    [HttpPost]
    [Route("RemoveSelfFromChannel")]
    public async Task<IActionResult> RemoveSelfFromChannel([FromBody] ChannelIdDto model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("User was not found.");
        }
        
        try
        {
            await _channelService.RemoveUserFromChannelAsync(model.channelId, user.Id);
            return Ok(new { message = "User successfully removed from channel" });
        }
        catch (Exception error)
        {
            return BadRequest("Error occured when trying to remove user from channel: " + error.Message);
        }
    }
}