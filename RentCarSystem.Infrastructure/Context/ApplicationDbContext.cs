using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Configurations;

namespace RentCarSystem.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
            
        {
        }

        // DbSetler - Tablolar
        public DbSet<User> Users { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleImage> VehicleImages { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RentalAgreement> RentalAgreements { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }
        public DbSet<DamageImage> DamageImages { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = "System"; 
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedBy = "System"; 
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
