namespace LicentaBackend.DTO
{
    public class CabinSearchResult
    {
        public string? Location { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? Guests { get; set; }
    }
}
