using LicentaBackend.Models;

namespace LicentaBackend.DTO
{
    public class ReservationRequestDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Country { get; set; } = "";
        public string Message { get; set; } = "";

        public int CabinId { get; set; }
        public string AccessPin { get; set; } = "";

        public DateTime? CheckIn { get; set; }   
        public DateTime? CheckOut { get; set; }  

        public Cabin? Cabin { get; set; }
    }
}
