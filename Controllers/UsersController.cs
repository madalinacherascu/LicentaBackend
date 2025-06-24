using LicentaBackend.DTO;
using LicentaBackend.Filters;
using LicentaBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> Get()
        {
            var user = await _context.Users.ToListAsync();
            return Ok(user);
        }
        
    }
}
