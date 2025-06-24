using Microsoft.AspNetCore.Mvc;
using LicentaBackend.Models;
using LicentaBackend.Filters;
using Microsoft.EntityFrameworkCore;
using LicentaBackend.DTO;
using Microsoft.Data.SqlClient;

using static Org.BouncyCastle.Math.EC.ECCurve;

namespace LicentaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CabinsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly IConfiguration _config;

        public CabinsController(AppDataBaseContext context, IConfiguration config )
        {
            _context = context;
            _config = config;
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
        public async Task<IActionResult> Search([FromQuery] CabinSearchResult request)
        {
            var results = new List<Cabin>();
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT * FROM Cabin
                WHERE (@Location IS NULL OR Location LIKE '%' + @Location + '%')
                  AND(@CheckIn IS NULL OR AvailableFrom IS NULL OR AvailableFrom <= @CheckIn)
                  AND (@CheckOut IS NULL OR AvailableTo IS NULL OR AvailableTo >= @CheckOut)
                  AND (@Guests IS NULL OR Capacity >= @Guests)
            ";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Location", (object?)request.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@CheckIn", request.CheckIn.HasValue ? request.CheckIn.Value : DBNull.Value);
            command.Parameters.AddWithValue("@CheckOut", request.CheckOut.HasValue ? request.CheckOut.Value : DBNull.Value);
            command.Parameters.AddWithValue("@Guests", request.Guests.HasValue ? request.Guests.Value : DBNull.Value);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new Cabin
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Location = reader.GetString(2),
                    Image = reader.GetString(3),
                    Capacity = reader.GetInt32(4),
                    Bedrooms = reader.GetInt32(5),
                    Price = reader.GetDecimal(6),
                    AvailableFrom = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                    AvailableTo = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                    Description = reader.IsDBNull(9) ? null : reader.GetString(9),
                    Amenities = reader.IsDBNull(10) ? null : reader.GetString(10),
                    CheckIn = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                    CheckOut = reader.IsDBNull(12) ? null : reader.GetDateTime(12)
                });
            }

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
        [HttpGet("reserved-dates/{id}")]
        public async Task<IActionResult> GetReservedDates(int id)
        {
            var reservedDates = await _context.ReservationRequests
                .Where(r => r.CabinId == id)
                .Select(r => new
                {
                    checkIn = r.CheckIn,
                    checkOut = r.CheckOut
                })
                .ToListAsync();

            return Ok(reservedDates);
        }






    }
}
