using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarSystem.Domain.Entities;

namespace RentCarSystem.Infrastructure.Configurations
{
    public class DamageImageConfiguration : IEntityTypeConfiguration<DamageImage>
    {
        public void Configure(EntityTypeBuilder<DamageImage> builder)
        {
            builder.ToTable("DamageImages");

            builder.HasKey(di => di.Id);

            builder.Property(di => di.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

           
            builder.HasOne(di => di.DamageReport)
                .WithMany(dr => dr.DamageImages)
                .HasForeignKey(di => di.DamageReportId) 
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}