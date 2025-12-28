using Microsoft.EntityFrameworkCore;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Security;

namespace RentCarSystem.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IAesEncryptionService _encryptionService;  // ✅ FIELD EKLENDI!

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IAesEncryptionService encryptionService)  // ✅ PARAMETRE EKLENDI!
            : base(options)
        {
            _encryptionService = encryptionService;  // ✅ ATAMA YAPILDI!
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
        public DbSet<PaymentRequest> PaymentRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== AES ŞİFRELEME CONVERTER ==========
            var encryptedConverter = new EncryptedStringConverter(_encryptionService);

            // User - Hassas bilgileri şifrele
            modelBuilder.Entity<User>()
                .Property(u => u.DriverLicenseNumber)
                .HasConversion(encryptedConverter);

            modelBuilder.Entity<User>()
                .Property(u => u.IdentityNumber)
                .HasConversion(encryptedConverter);

            modelBuilder.Entity<User>()
                .Property(u => u.PhoneNumber)
                .HasConversion(encryptedConverter);

            modelBuilder.Entity<User>()
                .Property(u => u.Address)
                .HasConversion(encryptedConverter);

            modelBuilder.Entity<User>()
                .Property(u => u.EmergencyContactPhone)
                .HasConversion(encryptedConverter);

            // ========== CONFIGURATION FILES ==========
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // ========== PAYMENTREQUEST ENTITY ==========
            modelBuilder.Entity<PaymentRequest>(entity =>
            {
                entity.ToTable("PaymentRequests");

                entity.HasIndex(e => e.IdempotencyKey)
                    .IsUnique()
                    .HasDatabaseName("IX_PaymentRequests_IdempotencyKey");

                entity.Property(e => e.IdempotencyKey)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.RequestBody)
                    .IsRequired();

                entity.Property(e => e.ResponseBody)
                    .IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Reservation)
                    .WithMany()
                    .HasForeignKey(e => e.ReservationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .ValueGeneratedOnAddOrUpdate();
            });

            // ========== PUBLICID CONFIGURATIONS ==========

            // Reservation
            modelBuilder.Entity<Reservation>()
                .Property(r => r.PublicId)
                .HasDefaultValueSql("NEWID()")
                .IsRequired();

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.PublicId)
                .IsUnique();

            // Payment
            modelBuilder.Entity<Payment>()
                .Property(p => p.PublicId)
                .HasDefaultValueSql("NEWID()")
                .IsRequired();

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.PublicId)
                .IsUnique();

            // User
            modelBuilder.Entity<User>()
                .Property(u => u.PublicId)
                .HasDefaultValueSql("NEWID()")
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PublicId)
                .IsUnique();

            // PaymentRequest
            modelBuilder.Entity<PaymentRequest>()
                .Property(p => p.PublicId)
                .HasDefaultValueSql("NEWID()")
                .IsRequired();

            modelBuilder.Entity<PaymentRequest>()
                .HasIndex(p => p.PublicId)
                .IsUnique();
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