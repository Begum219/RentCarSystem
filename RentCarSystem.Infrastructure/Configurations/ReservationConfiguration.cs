using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarSystem.Domain.Entities;

namespace RentCarSystem.Infrastructure.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");

            builder.HasKey(r => r.Id);

            // Fiyat alanları
            builder.Property(r => r.BasePrice)
                .HasPrecision(18, 2);

            builder.Property(r => r.ExtraServicesFee)
                .HasPrecision(18, 2);

            builder.Property(r => r.InsuranceFee)
                .HasPrecision(18, 2);

            builder.Property(r => r.ExtraKilometerFee)
                .HasPrecision(18, 2);

            builder.Property(r => r.FuelDifferenceFee)
                .HasPrecision(18, 2);

            builder.Property(r => r.LateFee)
                .HasPrecision(18, 2);

            builder.Property(r => r.DiscountAmount)
                .HasPrecision(18, 2);

            builder.Property(r => r.TotalPrice)
                .HasPrecision(18, 2);

            builder.Property(r => r.DepositAmount)
                .HasPrecision(18, 2);

            builder.Property(r => r.CancellationReason)
                .HasMaxLength(500);

            builder.Property(r => r.Notes)
                .HasMaxLength(1000);

            // Optimistic Locking
            builder.Property(r => r.RowVersion)
                .IsRowVersion();

            // İlişkiler
            builder.HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Vehicle)
                .WithMany(v => v.Reservations)
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Pickup Location İlişkisi
            builder.HasOne(r => r.PickupLocation)
                .WithMany(l => l.PickupReservations)
                .HasForeignKey(r => r.PickupLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            //Return Location İlişkisi
            builder.HasOne(r => r.ReturnLocation)
                .WithMany(l => l.ReturnReservations)
                .HasForeignKey(r => r.ReturnLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.Payments)
                .WithOne(p => p.Reservation)
                .HasForeignKey(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.RentalAgreement)
                .WithOne(ra => ra.Reservation)
                .HasForeignKey<RentalAgreement>(ra => ra.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Reviews)
                .WithOne(rev => rev.Reservation)
                .HasForeignKey(rev => rev.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}