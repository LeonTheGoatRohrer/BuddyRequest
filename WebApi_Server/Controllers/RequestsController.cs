using Microsoft.AspNetCore.Mvc;
using Models;
using ORM.Services;

namespace WebApi_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var db = new DbManager();

            var request = new Request
            {
                SenderId = dto.SenderId,
                ReceiverId = dto.ReceiverId,
                RequestType = dto.RequestType,
                Message = dto.Message
            };

            var createdRequest = await db.CreateRequestAsync(request);
            return Ok(createdRequest);
        }

        [HttpGet("pending/{userId}")]
        public async Task<IActionResult> GetPendingRequests(int userId)
        {
            using var db = new DbManager();
            var requests = await db.GetPendingRequestsForUserAsync(userId);
            return Ok(requests);
        }

        [HttpGet("sent/{userId}")]
        public async Task<IActionResult> GetSentRequests(int userId)
        {
            using var db = new DbManager();
            var requests = await db.GetSentRequestsAsync(userId);
            return Ok(requests);
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetRequestHistory(int userId)
        {
            using var db = new DbManager();
            var requests = await db.GetRequestHistoryAsync(userId);
            return Ok(requests);
        }

        [HttpPut("accept/{requestId}")]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            using var db = new DbManager();
            var success = await db.AcceptRequestAsync(requestId);

            if (!success)
            {
                return NotFound(new { message = "Request nicht gefunden oder bereits beantwortet" });
            }

            return Ok(new { message = "Request akzeptiert" });
        }

        [HttpPut("decline/{requestId}")]
        public async Task<IActionResult> DeclineRequest(int requestId)
        {
            using var db = new DbManager();
            var success = await db.DeclineRequestAsync(requestId);

            if (!success)
            {
                return NotFound(new { message = "Request nicht gefunden oder bereits beantwortet" });
            }

            return Ok(new { message = "Request abgelehnt" });
        }
    }
}
