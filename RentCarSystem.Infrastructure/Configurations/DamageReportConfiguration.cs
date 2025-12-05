using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarSystem.Domain.Entities;

namespace RentCarSystem.Infrastructure.Configurations
{
    public class DamageReportConfiguration : IEntityTypeConfiguration<DamageReport>
    {
        public void Configure(EntityTypeBuilder<DamageReport> builder)
        {
            builder.ToTable("DamageReports");

            builder.HasKey(dr => dr.Id);

            builder.Property(dr => dr.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(dr => dr.EstimatedCost)
                .HasPrecision(18, 2);

            builder.Property(dr => dr.ActualCost)
                .HasPrecision(18, 2);

            builder.Property(dr => dr.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(dr => dr.Resolution)
                .HasMaxLength(1000);

            // Optimistic Locking
            builder.Property(dr => dr.RowVersion)
                .IsRowVersion();

            // İlişkiler
            builder.HasOne(dr => dr.Reservation)
                .WithMany()
                .HasForeignKey(dr => dr.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dr => dr.RentalAgreement)
                .WithMany(ra => ra.DamageReports)
                .HasForeignKey(dr => dr.RentalAgreementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(dr => dr.DamageImages)
                .WithOne(di => di.DamageReport)
                .HasForeignKey(di => di.DamageReportId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}