using Microsoft.AspNetCore.Mvc;
using Models;
using ORM.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly DbManager _dbManager;

        public MessagesController(DbManager dbManager)
        {
            _dbManager = dbManager;
        }

        // POST: api/messages/send
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            if (dto == null || dto.SenderId <= 0 || dto.ReceiverId <= 0 || string.IsNullOrWhiteSpace(dto.Message))
            {
                return BadRequest(new { message = "Ungültige Nachricht." });
            }

            var message = new Messages
            {
                SenderId = dto.SenderId,
                EmpfaengerId = dto.ReceiverId,
                Message = dto.Message,
                SentAt = DateTime.UtcNow
            };

            var createdMessage = await _dbManager.SendeNachrichtAsync(message);
            return Ok(createdMessage);
        }

        // GET: api/messages/chat/{userId1}/{userId2}
        [HttpGet("chat/{userId1}/{userId2}")]
        public async Task<IActionResult> GetChatHistory(int userId1, int userId2)
        {
            if (userId1 <= 0 || userId2 <= 0)
            {
                return BadRequest(new { message = "Ungültige User-IDs." });
            }

            var messages = await _dbManager.GetChatHistoryAsync(userId1, userId2);
            return Ok(messages);
        }

        // GET: api/messages/conversations/{userId}
        [HttpGet("conversations/{userId}")]
        public async Task<IActionResult> GetConversations(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Ungültige User-ID." });
            }

            var conversations = await _dbManager.GetConversationsAsync(userId);
            return Ok(conversations);
        }
    }

    public class SendMessageDto
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Message { get; set; }
    }
}
