using LicentaBackend.Filters;
using LicentaBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;

namespace LicentaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationRequestController : ControllerBase
    {
        private readonly AppDataBaseContext _context;

        public ReservationRequestController(AppDataBaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationRequest reservation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           
            Random rnd = new Random();
            reservation.AccessPin = rnd.Next(1000, 10000).ToString();

           
            _context.ReservationRequests.Add(reservation);
            await _context.SaveChangesAsync();

            
            var tempFilePath = Path.Combine(Path.GetTempPath(), "pins_arduino.txt");
            await System.IO.File.AppendAllTextAsync(tempFilePath, reservation.AccessPin + Environment.NewLine);


           
            string esp32Ip = "http://172.20.10.2:236/send";

            try
            {
                using var client = new HttpClient();
                var content = new StringContent(reservation.AccessPin, Encoding.UTF8, "text/plain");
                var espResponse = await client.PostAsync(esp32Ip, content);

                if (!espResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"ESP32 a returnat o eroare: {await espResponse.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare la trimitere PIN către ESP32: " + ex.Message);
            }            

            return Ok(reservation);
        }

        [HttpGet("toate")]
        public async Task<IActionResult> GetToatePinele()
        {
            var pinuri = await _context.ReservationRequests
                            .Select(r => r.AccessPin)
                            .ToListAsync();

            return Ok(pinuri); 
        }

    }
}
