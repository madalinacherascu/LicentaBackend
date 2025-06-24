namespace LicentaBackend.DTO
{
    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public DateTime? DataNasterii { get; set; }
        public string? NumarTelefon { get; set; }
        public string? Nationalitate { get; set; }
        public string? Adresa { get; set; }

    }
}
