using ChatProject.Models;
using ChatProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _service;

        public MessageController(IMessageService service)
        {
            _service = service;
        }
        
        // GET: api/Message
        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _service.GetAllMessagesAsync();
            return messages.Any() ? Ok(messages) : NotFound();
        }
    }
}
