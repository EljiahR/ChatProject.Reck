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
        [Authorize]
        public IActionResult GetMessages()
        {
            var messages = _service.GetAllMessages();
            return messages.Any() ? Ok(messages) : NotFound();
        }

        // POST: api/Message
        [HttpPost]
        public IActionResult AddMessage(Message message)
        {
            try
            {
                _service.AddMessage(message);
                return Ok(message);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return BadRequest("Problem with message");
            }
        }
    }
}
