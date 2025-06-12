namespace LicentaBackend.DTO
{
    public class CabinDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Image { get; set; }
        public int Capacity { get; set; }
        public int Bedrooms { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }

        public List<string> Images { get; set; } = new();
        public string? Amenities { get; set; }
    }

}
