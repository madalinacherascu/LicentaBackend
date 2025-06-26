using LicentaBackend.Filters;
using LicentaBackend.Models;
using LicentaBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace LicentaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationRequestController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly EmailService _emailService;

        public ReservationRequestController(AppDataBaseContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var userGuid = Guid.Parse(userId); 

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userGuid); 


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
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationRequest reservation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
            bool alreadyReserved = await _context.ReservationRequests.AnyAsync(r =>
                r.CabinId == reservation.CabinId &&
                reservation.CheckIn < r.CheckOut &&
                reservation.CheckOut > r.CheckIn
            );

            if (alreadyReserved)
                return Conflict(new { message = "Cabana este deja rezervată în perioada selectată." });

            
            reservation.AccessPin = new Random().Next(1000, 10000).ToString();

            
            _context.ReservationRequests.Add(reservation);
            await _context.SaveChangesAsync();

   
            var cabin = await _context.Cabin.FindAsync(reservation.CabinId);
            if (cabin != null)
            {
                cabin.CheckIn = reservation.CheckIn;
                cabin.CheckOut = reservation.CheckOut;
                _context.Cabin.Update(cabin);
                await _context.SaveChangesAsync();
            }

            //string esp32Ip = "http://172.20.10.2:236/send";
            
            //    using var client = new HttpClient();
            //    var content = new StringContent(reservation.AccessPin, Encoding.UTF8, "text/plain");
            //    await client.PostAsync(esp32Ip, content);


            return Ok(reservation);
        }



        [HttpGet("toate")]
        public async Task<IActionResult> GetToatePinurile()
        {
            var pinuri = await _context.ReservationRequests
                            .Select(r => r.AccessPin)
                            .ToListAsync();

            return Ok(pinuri);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _context.ReservationRequests.ToListAsync();
            return Ok(reservations);
        }
       
    }
}
