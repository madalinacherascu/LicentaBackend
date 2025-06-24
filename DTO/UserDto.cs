namespace LicentaBackend.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }  
        public string Name { get; set; }
        public string Email { get; set; }
        public bool RememberMe { get; set; }
    }
}
