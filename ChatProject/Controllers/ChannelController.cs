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
    private readonly UserManager<ChatUser> _userManager;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ConnectionManager _connectionManager;

    public ChannelController(IChannelService channelService, UserManager<ChatUser> userManager, IHubContext<ChatHub> hubContext, ConnectionManager connectionManager)
    {
        _channelService = channelService;
        _userManager = userManager;
        _hubContext = hubContext;
        _connectionManager = connectionManager;
    }

    [HttpPost]
    [Route("New")]
    public async Task<IActionResult> CreateChannel([FromBody] NewChannelDto model)
    {
        var user = await _userManager.GetUserAsync(User);
        var newChannel = new ChatChannel { Name = model.Name, CreatedBy = user!.Id};
        
        var newId = await _channelService.AddChannelAsync(newChannel);
        user.ChannelIds.Add(newId);
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
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized("User was not found.");
        }

        if (!user.ChannelIds.Contains(channelId))
        {
            return Unauthorized("User not a member of the channel.");
        }

        var newChannelUser = await _userManager.FindByIdAsync(userId);

        if (newChannelUser == null)
        {
            return NotFound($"User with id '{userId}' was not found.");
        } else if (newChannelUser.ChannelIds.Contains(channelId))
        {
            return BadRequest("User already in channel.");
        }

        try
        {
            await _channelService.AddMemberToChannelAsync(channelId, userId);
            newChannelUser.ChannelIds.Add(channelId);
            await _userManager.UpdateAsync(newChannelUser);

            return Ok("User added to channel successfully!");
        }
        catch (Exception error)
        {
            return BadRequest("Error occured when adding user: " + error.Message);
        }
    }
}