using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarSystem.Domain.Entities;

namespace RentCarSystem.Infrastructure.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.Address)
                .IsRequired()
                .HasMaxLength(500);  

            builder.Property(l => l.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.District)
                .HasMaxLength(100);

            builder.Property(l => l.Latitude)
                .HasPrecision(9, 6);

            builder.Property(l => l.Longitude)
                .HasPrecision(9, 6);

            builder.Property(l => l.PhoneNumber)
                .HasMaxLength(20);  

            builder.Property(l => l.Email)
                .HasMaxLength(256); 

         
        }
    }
}