using Microsoft.AspNetCore.Mvc;
using LicentaBackend.Models;
using LicentaBackend.Filters;
using Microsoft.EntityFrameworkCore;

namespace LicentaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestimonialsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;

        public TestimonialsController(AppDataBaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Testimonials>>> Get()
        
        {
            var testimonials = await _context.Testimonials.ToListAsync();
            return Ok(testimonials);
        }

    }
}
