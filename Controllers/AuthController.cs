using LicentaBackend.DTO;
using LicentaBackend.Filters;
using LicentaBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            if (user.RememberMe != dto.RememberMe)
            {
                user.RememberMe = dto.RememberMe;
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.RememberMe
            });
        }
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                name = user.Name,
                email = user.Email
            });
        }

        //[HttpPost("forgot-password")]
        //public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.Email))
        //        return BadRequest("Emailul este obligatoriu.");

        //    var result = await _authService.SendPasswordResetLinkAsync(request.Email);

        //    // Nu divulgăm dacă emailul există sau nu
        //    return Ok(new { message = "Dacă adresa există în sistem, veți primi un email cu instrucțiuni." });
        //}
    }
}
