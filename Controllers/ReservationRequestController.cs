using LicentaBackend.Filters;
using LicentaBackend.Models;
using LicentaBackend.Services;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Diagnostics;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


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





            Random rnd = new Random();
            reservation.AccessPin = rnd.Next(1000, 10000).ToString();

           
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
            //var htmlBody = $@"
            //<h2>Bună, {reservation.Name}!</h2>
            //<p>Rezervarea ta pentru <strong>{reservation.CabinName}</strong> a fost înregistrată.</p>
            //<ul>
            //    <li><strong>Locație:</strong> {reservation.CabinLocation}</li>
            //    <li><strong>Capacitate:</strong> {reservation.CabinCapacity} persoane</li>
            //    <li><strong>Dormitoare:</strong> {reservation.CabinBedrooms}</li>
            //    <li><strong>Preț/noapte:</strong> {reservation.CabinPrice} lei</li>
            //    <li><strong>Total:</strong> {reservation.TotalPrice} lei</li>
            //    <li><strong>Check-in:</strong> {reservation.CheckIn}</li>
            //    <li><strong>Check-out:</strong> {reservation.CheckOut:dd.MM.yyyy}</li>
            //</ul>
            //<h3>PIN de acces: <code>{reservation.AccessPin}</code></h3>
            //<p>Mulțumim că ai ales Cabane Montane!</p>";

            //await _emailService.SendReservationEmailAsync(
            //    reservation.Email,
            //    "Confirmare rezervare cabană",
            //    htmlBody
            //);

            return Ok(reservation);
        }
        public async Task SendReservationEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Cabane Montane", "noreply@cabanemontane.ro"));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.mail.yahoo.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("cabanemontane", "password");
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }


        [HttpGet("toate")]
        public async Task<IActionResult> GetToatePinele()
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
