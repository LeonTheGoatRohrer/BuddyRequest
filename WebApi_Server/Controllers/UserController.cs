using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Models;
using ORM.Services;
using System.Threading.Tasks;

namespace WebApi_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DbManager _dbManager;
        private readonly IPasswordHasher<User> passwordHasher;

        //DI
        public UsersController(DbManager dbManager, IPasswordHasher<User> passwordHasher)
        {
            this._dbManager = dbManager;
            this.passwordHasher = passwordHasher;
        }

        //GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await this._dbManager.findeBenutzerNachIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = $"User mit ID {id} nicht gefunden." });
            }

            return Ok(user);
        }

        //GET: api/users/byKey/AB1234
        [HttpGet("byKey/{key}")]
        public async Task<IActionResult> GetUserByKey(string key)
        {
            var user = await this._dbManager.findeBenutzerNachSchluesselAsync(key);

            if (user == null)
            {
                return NotFound(new { message = $"Kein User mit Key '{key}' gefunden." });
            }

            return Ok(user);
        }

        //GET: api/users/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Ungültige User-ID." });
            }

            var users = await this._dbManager.GetAllUsersAsync(userId);
            return Ok(users);
        }

        //POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest(new { message = "Username und Passwort dürfen nicht leer sein." });
            }

            //existiert user bereits?
            var existingUser = await this._dbManager.findeBenutzerNachNameAsync(user.Username);

            if (existingUser != null)
            {
                return Conflict(new { message = "Dieser Username ist bereits vergeben." });
            }

            var hashedPassword = this.passwordHasher.HashPassword(user, user.Password);
            user.Password = hashedPassword;

            //existiert key? -> generieren
            if (string.IsNullOrWhiteSpace(user.Key))
            {
                user.Key = ErzeugeUserKey(user.Username);
            }

            var createdUser = await this._dbManager.registriereBenutzerAsync(user);

            return CreatedAtAction(nameof(GetUserById), new {id = createdUser.Id}, createdUser);
        }

        //POST: api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDaten)
        {
            if (loginDaten == null || string.IsNullOrWhiteSpace(loginDaten.Username) || string.IsNullOrWhiteSpace(loginDaten.Password))
            {
                return BadRequest(new { message = "Username und Passwort dürfen nicht leer sein." });
            }

            var user = await this._dbManager.findeBenutzerNachNameAsync(loginDaten.Username);

            if (user == null)
            {
                return Unauthorized(new { message = "Benutzer existiert nicht." });
            }

            var result = this.passwordHasher.VerifyHashedPassword(user, user.Password, loginDaten.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Passwort ist falsch." });
            }

            return Ok(user);
        }

        // PATCH: api/users/update-bio/{id}
        [HttpPatch("update-bio/{id}")]
        public async Task<IActionResult> UpdateBio(int id, [FromBody] BioDto dto)
        {
            var user = await this._dbManager.findeBenutzerNachIdAsync(id);
            if (user == null) return NotFound();
            user.Bio = dto.Bio;
            await this._dbManager.saveToDbAsync();
            return Ok();
        }

        // PATCH: api/users/update-avatar/{id}
        [HttpPatch("update-avatar/{id}")]
        public async Task<IActionResult> UpdateAvatar(int id)
        {
            var user = await this._dbManager.findeBenutzerNachIdAsync(id);
            if (user == null) return NotFound();
            
            // Neuen zufälligen Avatar generieren
            var randomSeed = Guid.NewGuid().ToString();
            user.AvatarUrl = $"https://api.dicebear.com/7.x/avataaars/png?seed={randomSeed}";
            
            await this._dbManager.saveToDbAsync();
            return Ok(new { avatarUrl = user.AvatarUrl });
        }

        // PATCH: api/users/update-profile/{id}
        [HttpPatch("update-profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] ProfileDto dto)
        {
            var user = await this._dbManager.findeBenutzerNachIdAsync(id);
            if (user == null) return NotFound();

            // Prüfen ob neuer Username bereits vergeben
            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.Username)
            {
                var existing = await this._dbManager.findeBenutzerNachNameAsync(dto.Username);
                if (existing != null)
                    return Conflict(new { message = "Username bereits vergeben." });
                user.Username = dto.Username;
            }

            // Email aktualisieren
            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            await this._dbManager.saveToDbAsync();
            return Ok(user);
        }

        private string ErzeugeUserKey(string username)
        {
            var random = new System.Random(); //Zufallszahl
            int number = random.Next(1000, 9999); //Zufallszahl

            string prefix = username.Length >= 2 ? username.Substring(0, 2).ToUpper() : "US"; //Prefix aus Username nehmen -> wenn zu klein dann "US"

            return $"{prefix}{number}"; //Schlüssen zusammenbauen und zurückgeben
        }

        // POST: api/users/share-location/{id}
        [HttpPost("share-location/{id}")]
        public async Task<IActionResult> ShareLocation(int id, [FromBody] LocationDto dto)
        {
            var user = await this._dbManager.findeBenutzerNachIdAsync(id);
            if (user == null) return NotFound(new { message = "User nicht gefunden." });

            user.Latitude = dto.Latitude;
            user.Longitude = dto.Longitude;
            user.LastLocationUpdate = DateTime.UtcNow;

            await this._dbManager.saveToDbAsync();
            return Ok(new { message = "Location aktualisiert.", latitude = user.Latitude, longitude = user.Longitude });
        }

        // GET: api/users/friends-locations/{id}
        [HttpGet("friends-locations/{id}")]
        public async Task<IActionResult> GetFriendsLocations(int id)
        {
            try
            {
                var user = await this._dbManager.findeBenutzerNachIdAsync(id);
                if (user == null) return NotFound();

                // Hole alle Freunde des Users aus der Friends-Tabelle
                var friendIds = this._dbManager.Friends
                    .Where(f => (f.UserId == id || f.FriendUserId == id) && f.Angenommen)
                    .Select(f => f.UserId == id ? f.FriendUserId : f.UserId)
                    .ToList();

                // Hole die User-Daten mit Location-Info
                var friendLocations = this._dbManager.Users
                    .Where(u => friendIds.Contains(u.Id) && u.Latitude.HasValue && u.Longitude.HasValue)
                    .Select(u => new 
                    { 
                        u.Id, 
                        u.Username, 
                        u.Latitude, 
                        u.Longitude, 
                        u.LastLocationUpdate,
                        u.AvatarUrl
                    })
                    .ToList();

                return Ok(friendLocations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/users/my-location/{id}
        [HttpGet("my-location/{id}")]
        public async Task<IActionResult> GetMyLocation(int id)
        {
            var user = await this._dbManager.findeBenutzerNachIdAsync(id);
            if (user == null) return NotFound();

            return Ok(new 
            { 
                id = user.Id,
                username = user.Username,
                latitude = user.Latitude,
                longitude = user.Longitude,
                lastLocationUpdate = user.LastLocationUpdate,
                avatarUrl = user.AvatarUrl
            });
        }

        public class BioDto
        {
            public string Bio { get; set; }
        }

        public class ProfileDto
        {
            public string? Username { get; set; }
            public string? Email { get; set; }
        }

        public class LoginDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class LocationDto
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}