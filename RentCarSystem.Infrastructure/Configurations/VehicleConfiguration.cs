using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarSystem.Domain.Entities;

namespace RentCarSystem.Infrastructure.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.PlateNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(v => v.ChassisNumber)
                .HasMaxLength(50);

            builder.Property(v => v.Color)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.DailyPrice)
                .HasPrecision(18, 2);

            builder.Property(v => v.HourlyPrice)
                .HasPrecision(18, 2);

            builder.Property(v => v.WeeklyPrice)
                .HasPrecision(18, 2);

            builder.Property(v => v.MonthlyPrice)
                .HasPrecision(18, 2);

            builder.Property(v => v.DepositAmount)
                .HasPrecision(18, 2);

            builder.Property(v => v.ExtraKilometerFee)
                .HasPrecision(18, 2);

            builder.HasIndex(v => v.PlateNumber)
                .IsUnique();

            // Optimistic Locking
            builder.Property(v => v.RowVersion)
                .IsRowVersion();

            // İlişkiler
            builder.HasOne(v => v.Brand)
                .WithMany(b => b.Vehicles)
                .HasForeignKey(v => v.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Category)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.VehicleImages)
                .WithOne(vi => vi.Vehicle)
                .HasForeignKey(vi => vi.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.Reservations)
                .WithOne(r => r.Vehicle)
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.Maintenances)
                .WithOne(m => m.Vehicle)
                .HasForeignKey(m => m.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.Insurances)
                .WithOne(i => i.Vehicle)
                .HasForeignKey(i => i.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}