using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarSystem.Domain.Entities;

namespace RentCarSystem.Infrastructure.Configurations
{
    public class VehicleImageConfiguration : IEntityTypeConfiguration<VehicleImage>
    {
        public void Configure(EntityTypeBuilder<VehicleImage> builder)
        {
            builder.ToTable("VehicleImages");

            builder.HasKey(vi => vi.Id);

            builder.Property(vi => vi.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            // İlişkiler
            builder.HasOne(vi => vi.Vehicle)
                .WithMany(v => v.VehicleImages)
                .HasForeignKey(vi => vi.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}