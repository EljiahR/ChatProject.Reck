using ChatProject.Models;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatProject.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class ChannelController : ControllerBase
{
    private readonly IChannelService _service;

    public ChannelController(IChannelService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("New")]
    public async Task<IActionResult> CreateChannel(string channelName)
    {
        var newChannel = new ChatChannel { Name = channelName };
        await _service.AddChannelAsync(newChannel);
        return Ok(newChannel.Id);
    }
}