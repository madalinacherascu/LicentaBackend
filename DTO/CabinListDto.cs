namespace LicentaBackend.DTO
{
    public class CabinListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Image { get; set; }
        public int Capacity { get; set; }
        public int Bedrooms { get; set; }
        public decimal Price { get; set; }
    }
}
