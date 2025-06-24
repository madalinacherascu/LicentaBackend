namespace LicentaBackend.DTO
{
    public class UpdateUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Adresa { get; set; }
        public string? Nationalitate { get; set; }
        public string? NumarTelefon { get; set; }
        public DateTime? DataNasterii { get; set; }
    }
}
