using Microsoft.AspNetCore.Mvc;
using LicentaBackend.Models;
using LicentaBackend.Filters;
using Microsoft.EntityFrameworkCore;
using LicentaBackend.DTO;

namespace LicentaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CabinsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;

        public CabinsController(AppDataBaseContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CabinListDto>>> Get()
        {
            var cabins = await _context.Cabin
                .Select(c => new CabinListDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    Image = c.Image,
                    Capacity = c.Capacity,
                    Bedrooms = c.Bedrooms,
                    Price = c.Price
                })
                .ToListAsync();

            return Ok(cabins);
        }


        [HttpGet("search")]
        public IActionResult Search(string location, DateTime? checkIn, DateTime? checkOut, int? guests)
        {
            var query = _context.Cabin.AsQueryable();

            if (!string.IsNullOrEmpty(location))
                query = query.Where(c => c.Location.ToLower().Contains(location.ToLower()));

            if (checkIn.HasValue)
            {
                query = query.Where(c =>
                    !c.AvailableFrom.HasValue || c.AvailableFrom <= checkIn.Value);
            }

            if (checkOut.HasValue)
            {
                query = query.Where(c =>
                    !c.AvailableTo.HasValue || c.AvailableTo >= checkOut.Value);
            }

            if (guests.HasValue)
            {
                query = query.Where(c => c.Capacity >= guests.Value);
            }

            var results = query.ToList();
            return Ok(results);
        }


        [HttpGet("{id}")]
        public IActionResult GetCabinById(int id)
        {
            var cabin = _context.Cabin
                .Include(c => c.Images)
                .FirstOrDefault(c => c.Id == id);

            if (cabin == null)
                return NotFound();

            var result = new CabinDetailsDto
            {
                Id = cabin.Id,
                Name = cabin.Name,
                Location = cabin.Location,
                Image = cabin.Image,
                Capacity = cabin.Capacity,
                Bedrooms = cabin.Bedrooms,
                Price = cabin.Price,
                Description = cabin.Description,
                AvailableFrom = cabin.AvailableFrom,
                AvailableTo = cabin.AvailableTo,
                Amenities = cabin.Amenities,
                Images = cabin.Images.Select(img => img.ImageUrl).ToList()

            };

            return Ok(result);
        }



    }
}
