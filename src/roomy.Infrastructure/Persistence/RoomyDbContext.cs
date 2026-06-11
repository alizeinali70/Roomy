using Microsoft.EntityFrameworkCore;
using roomy.Domain.Entities;

namespace roomy.Infrastructure.Persistence
{
    public class RoomyDbContext : DbContext
    {
        public RoomyDbContext(DbContextOptions<RoomyDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Office> Offices { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.HasMany(e => e.Bookings)
                    .WithOne(b => b.User)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Office>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Location).HasMaxLength(255).IsRequired();
                entity.HasMany(e => e.Bookings)
                    .WithOne(b => b.Office)
                    .HasForeignKey(b => b.OfficeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.OfficeId, e.Date });
                entity.Property(e => e.Date).IsRequired();
            });

            var adminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = adminUserId,
                Email = "admin@roomy.local",
                PasswordHash = HashPassword("admin123"),
                Role = Role.Admin,
                CreatedAt = DateTime.UtcNow
            });
        }

        private static string HashPassword(string password)
        {
            var hashedBytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
