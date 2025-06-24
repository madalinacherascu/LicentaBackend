using LicentaBackend.DTO;
using LicentaBackend.Filters;
using LicentaBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LicentaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        public UsersController(AppDataBaseContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Users>>> Get()
        //{
        //    var user = await _context.Users.ToListAsync();
        //    return Ok(user);
        //}

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var userGuid = Guid.Parse(userId); // ✅ corect

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userGuid); // acum e Guid == Guid


            return Ok(new
            {
                user.Name,
                user.Email,
                user.DataNasterii,
                user.NumarTelefon,
                user.Nationalitate,
                user.Adresa
            });
        }


    }
}
