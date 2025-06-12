using LicentaBackend.Models;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;


namespace LicentaBackend.Filters
{
    public class AppDataBaseContext : DbContext
    {
        public AppDataBaseContext(DbContextOptions<AppDataBaseContext> options) : base(options) { }

        public DbSet<Testimonials> Testimonials { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Cabin> Cabin { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<ReservationRequest> ReservationRequests { get; set; }
        public DbSet<CabinImages> CabinImages { get; set; }

    }
}
