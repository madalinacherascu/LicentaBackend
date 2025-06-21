using System.ComponentModel.DataAnnotations;

namespace LicentaBackend.Models
{
    public class ReservationRequest
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int CabinId { get; set; }
        public string CabinName { get; set; } = string.Empty;
        public string CabinLocation { get; set; } = string.Empty;
        public int CabinCapacity { get; set; }
        public int CabinBedrooms { get; set; }
        public decimal CabinPrice { get; set; }
        public string? AccessPin { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
