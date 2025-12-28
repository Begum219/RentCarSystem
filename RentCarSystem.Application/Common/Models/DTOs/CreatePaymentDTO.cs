using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class CreatePaymentDTO
    {
        [Required]
        public int ReservationId { get; set; }
        [Required]
        [Range(0, double.MaxValue)]

        public decimal Amount { get; set; }
        [Required]
        public int PaymentMethod { get; set; }  // enumda tanımladık
        [Required]
        public int PaymentType { get; set; }  // enumda tanımladık
        [StringLength(100)]
        public string? TransactionId { get; set; }
        public int Status { get; set; } = 2;
    }
}
