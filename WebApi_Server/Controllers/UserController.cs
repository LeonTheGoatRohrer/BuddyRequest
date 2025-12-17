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
        public async Task<IActionResult> Login([FromBody] User loginDaten)
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




        private string ErzeugeUserKey(string username)
        {
            var random = new System.Random(); //Zufallszahl
            int number = random.Next(1000, 9999); //Zufallszahl

            string prefix = username.Length >= 2 ? username.Substring(0, 2).ToUpper() : "US"; //Prefix aus Username nehmen -> wenn zu klein dann "US"

            return $"{prefix}{number}"; //Schlüssen zusammenbauen und zurückgeben
        }
    }
}