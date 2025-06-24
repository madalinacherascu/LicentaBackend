using LicentaBackend.DTO;
using LicentaBackend.Filters;
using LicentaBackend.Models;
using LicentaBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LicentaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly AppDataBaseContext _context;
        private readonly IPasswordHasher<Users> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthController(AuthService authService,AppDataBaseContext context, IPasswordHasher<Users> passwordHasher, IConfiguration configuration)
        {
            _authService = authService;
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }
        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            // Verifică dacă emailul este deja folosit
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
                return false;

            var user = new Users
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                RememberMe = dto.RememberMe,
                DataNasterii = dto.DataNasterii,
                NumarTelefon = dto.NumarTelefon,
                Nationalitate = dto.Nationalitate,
                Adresa = dto.Adresa
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
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

            var user = await _authService.AuthenticateAsync(dto); // ✅ lipsit în codul tău actual

            if (user == null)
                return Unauthorized("Email sau parolă incorectă.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = jwt,
                user.Id,
                user.Name,
                user.Email,
                user.RememberMe
            });
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateUserDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var userGuid = Guid.Parse(userId);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userGuid);
            if (user == null) return NotFound();

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.Adresa = dto.Adresa;
            user.Nationalitate = dto.Nationalitate;
            user.NumarTelefon = dto.NumarTelefon;
            user.DataNasterii = dto.DataNasterii;

            await _context.SaveChangesAsync();
            return Ok();
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
    }
}
