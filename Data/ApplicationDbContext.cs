using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TripManager.Models;

namespace TripManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<Landmark> Landmarks { get; set; }
        public DbSet<UserTour> UserTours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ensure Identity configurations are applied

            modelBuilder.Entity<UserTour>()
                .HasKey(ut => new { ut.UserId, ut.TourId });

            modelBuilder.Entity<UserTour>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTours)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<UserTour>()
                .HasOne(ut => ut.Tour)
                .WithMany(t => t.UserTours)
                .HasForeignKey(ut => ut.TourId);
        }
    }
}
