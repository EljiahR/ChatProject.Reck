using ChatProject.Models;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _service;
        private readonly UserManager<ChatUser> _userManager;
        public MessageController(IMessageService service, UserManager<ChatUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }
        
        // GET: /Message/ChatStarter
        [HttpGet]
        [Route("ChatStarter")]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _service.GetAllMessagesAsync();
            var user = await _userManager.GetUserAsync(User);
            
            return Ok(new { username = user!.UserName, messages });
        }
    }
}
