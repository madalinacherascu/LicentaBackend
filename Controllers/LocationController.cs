using Microsoft.AspNetCore.Mvc;
using LicentaBackend.Models;
using LicentaBackend.Filters;
using Microsoft.EntityFrameworkCore;

namespace LicentaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly AppDataBaseContext _context;

        public LocationController(AppDataBaseContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> Get()

        {
            var locations = await _context.Location.ToListAsync();
            return Ok(locations);
        }
    }
}

