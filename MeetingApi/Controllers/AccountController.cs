using MeetingApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // Lista korisnika u memoriji
        private static List<User> _users = new List<User>();

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Proveri da li korisnik sa ovim emailom već postoji
                if (_users.Any(u => u.Email == model.Email))
                {
                    return BadRequest("Email is already in use.");
                }

                // Kreiraj novog korisnika i dodaj ga u listu
                var user = new User { Name = model.Name, Email = model.Email, Password = model.Password };
                _users.Add(user);

                return Ok(new { result = "Registration successful" });
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Proveri da li postoji korisnik sa unetim emailom i lozinkom
                var user = _users.SingleOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    return Ok(new { result = "Login successful" });
                }

                return Unauthorized("Invalid email or password.");
            }

            return BadRequest(ModelState);
        }
    }

    public class RegisterModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
