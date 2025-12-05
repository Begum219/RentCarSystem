using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarSystem.Domain.Entities;

namespace RentCarSystem.Infrastructure.Configurations
{
    public class InsuranceConfiguration : IEntityTypeConfiguration<Insurance>
    {
        public void Configure(EntityTypeBuilder<Insurance> builder)
        {
            builder.ToTable("Insurances");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.PolicyNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.Provider)
                .IsRequired()
                .HasMaxLength(200); 

            builder.Property(i => i.CoverageType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.Premium)
                .HasPrecision(18, 2);

            builder.HasIndex(i => i.PolicyNumber)
                .IsUnique();

            // Optimistic Locking
            builder.Property(i => i.RowVersion)
                .IsRowVersion();

            // İlişkiler
            builder.HasOne(i => i.Vehicle)
                .WithMany(v => v.Insurances)
                .HasForeignKey(i => i.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}