using LicentaBackend.DTO;
using LicentaBackend.Filters;
using LicentaBackend.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace LicentaBackend.Services

{
    public class AuthService
    {
        private readonly AppDataBaseContext _context;

        public AuthService(AppDataBaseContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                return false;

            var user = new Users
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Users?> AuthenticateAsync(LoginDto dto)
        {
            var passwordHash = HashPassword(dto.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.PasswordHash == passwordHash);

            return user;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
