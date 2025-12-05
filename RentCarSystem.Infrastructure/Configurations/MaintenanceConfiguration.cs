using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Infrastructure.Configurations
{
    public class MaintenanceConfiguration : IEntityTypeConfiguration<Maintenance>
    {
        public void Configure(EntityTypeBuilder<Maintenance> builder)
        {
            builder.ToTable("Maintenances");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.MaintenanceType)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(m => m.Cost)
                .HasPrecision(18, 2);

            builder.Property(m => m.Description)
                .HasMaxLength(1000);
            builder.Property(m=>m.ServiceProvider)
                .HasMaxLength (200);

            //Optimistic locking
            builder.Property(m => m.RowVersion)
                .IsRowVersion();
            //ilişkiler
            builder.HasOne(m=>m.Vehicle)
                .WithMany(v=>v.Maintenances)
                .HasForeignKey(m => m.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        
        
        }
    }
}
