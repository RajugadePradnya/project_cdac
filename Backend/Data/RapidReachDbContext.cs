using Microsoft.EntityFrameworkCore;
using RapidReachApi.Models;

namespace RapidReachApi.Data
{
    public class RapidReachDbContext : DbContext
    {
        public RapidReachDbContext(DbContextOptions<RapidReachDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Shipment> Shipments { get; set; }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }


    }
}
