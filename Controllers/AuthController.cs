using LicentaBackend.DTO;
using LicentaBackend.Filters;
using LicentaBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LicentaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly AppDataBaseContext _context;

        public AuthController(AuthService authService,AppDataBaseContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Toate câmpurile sunt obligatorii.");
            }

            var success = await _authService.RegisterAsync(dto);
            if (!success)
                return Conflict("Emailul este deja folosit.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            return Ok(new
            {
                user = new
                {
                    user.Id,
                    user.Name,
                    user.Email
                }
            });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Emailul și parola sunt obligatorii.");

            var user = await _authService.AuthenticateAsync(dto);

            if (user == null)
                return Unauthorized("Email sau parolă incorectă.");

            
            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email
            });
        }
    }
}
