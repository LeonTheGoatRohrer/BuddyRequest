using Microsoft.AspNetCore.Mvc;
using Models;
using ORM.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly DbManager _dbManager;

        public FriendsController(DbManager dbManager)
        {
            _dbManager = dbManager;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Suchbegriff darf nicht leer sein." });
            }

            var users = await _dbManager.SucheBenutzerAsync(query);
            return Ok(users);
        }

        [HttpPost("request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDto request)
        {
            if (request == null || request.UserId <= 0 || request.FriendUserId <= 0)
            {
                return BadRequest(new { message = "Ungültige Anfrage." });
            }

            if (request.UserId == request.FriendUserId)
            {
                return BadRequest(new { message = "Du kannst dir nicht selbst eine Anfrage senden." });
            }

            var friendRequest = await _dbManager.SendeFriendRequestAsync(request.UserId, request.FriendUserId);

            if (friendRequest == null)
            {
                return Conflict(new { message = "Anfrage existiert bereits oder ihr seid bereits befreundet." });
            }

            return Ok(new { message = "Freundschaftsanfrage erfolgreich gesendet.", request = friendRequest });
        }

        [HttpGet("pending/{userId}")]
        public async Task<IActionResult> GetPendingRequests(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Ungültige User-ID." });
            }

            var requests = await _dbManager.GetPendingRequestsWithUserAsync(userId);
            return Ok(requests);
        }

        [HttpGet("list/{userId}")]
        public async Task<IActionResult> GetFriends(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Ungültige User-ID." });
            }

            var friends = await _dbManager.GetFreundeAsync(userId);
            return Ok(friends);
        }

        [HttpPut("accept/{requestId}")]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            if (requestId <= 0)
            {
                return BadRequest(new { message = "Ungültige Request-ID." });
            }

            var success = await _dbManager.AcceptFriendRequestAsync(requestId);

            if (!success)
            {
                return NotFound(new { message = "Anfrage nicht gefunden." });
            }

            return Ok(new { message = "Freundschaftsanfrage akzeptiert." });
        }

        [HttpDelete("decline/{requestId}")]
        public async Task<IActionResult> DeclineRequest(int requestId)
        {
            if (requestId <= 0)
            {
                return BadRequest(new { message = "Ungültige Request-ID." });
            }

            var success = await _dbManager.DeclineFriendRequestAsync(requestId);

            if (!success)
            {
                return NotFound(new { message = "Anfrage nicht gefunden." });
            }

            return Ok(new { message = "Freundschaftsanfrage abgelehnt." });
        }
    }

    public class FriendRequestDto
    {
        public int UserId { get; set; }
        public int FriendUserId { get; set; }
    }
}
