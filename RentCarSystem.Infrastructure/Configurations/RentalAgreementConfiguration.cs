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
    public class RentalAgreementConfiguration : IEntityTypeConfiguration<RentalAgreement>

    {
        public void Configure(EntityTypeBuilder<RentalAgreement> builder)
        {
            builder.ToTable("RentalAgreements");

            builder.HasKey(ra => ra.Id);

            builder.Property(ra => ra.ContractNumber)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(ra=>ra.PickupNotes)
                .HasMaxLength(1000);
            builder.Property(ra => ra.ReturnNotes)
                .HasMaxLength(1000);
            builder.Property(ra=>ra.CustomerSignatureUrl)
                .HasMaxLength (500);
            builder.Property(ra=>ra.StaffSignatureUrl)
                .HasMaxLength (500);
            
            builder.Property(ra => ra.ContractNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(ra => ra.ContractNumber)
                .IsUnique(); // IsUnique, Index üzerinde

            //optimistic locking
            builder.Property(ra => ra.RowVersion)
                .IsRowVersion();
            //ilişkiler
            builder.HasOne(ra=>ra.Reservation)
                .WithOne(r=>r.RentalAgreement)
                .HasForeignKey<RentalAgreement>(ra=>ra.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(ra=>ra.DamageReports)
                .WithOne(dr=>dr.RentalAgreement)
                .HasForeignKey(dr=>dr.RentalAgreementId)
                .OnDelete(DeleteBehavior.Restrict);  




        
        
        
        }

    }
}
