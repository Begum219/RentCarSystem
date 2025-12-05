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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder) {

            builder.ToTable("Payments");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasPrecision(18, 2);
            builder.Property(p => p.TransactionId)
                .HasMaxLength(200);
            builder.Property(p=>p.FailureReason)
                .HasMaxLength(500);
            //optimistic locking
            builder.Property(p => p.RowVersion)
                .IsRowVersion();

            //ilişkiler
            builder.HasOne(p=>p.Reservation)
                .WithMany(r=>r.Payments)
                .HasForeignKey(p=>p.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);
        
        
        
        
        
        }
    }
}
