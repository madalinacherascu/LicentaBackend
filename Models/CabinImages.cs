namespace LicentaBackend.Models
{
    public class CabinImages
    {
        public int Id { get; set; }

        public int CabinID { get; set; } 

        public string ImageUrl { get; set; }

        public Cabin? Cabin { get; set; }
    }

}
