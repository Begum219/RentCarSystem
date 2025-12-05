using RentCarSystem.Domain.Common;
using RentCarSystem.Domain.Enums;

namespace RentCarSystem.Domain.Entities
{
    public class Payment : BaseAuditableEntity
    {
        public int ReservationId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentType PaymentType { get; set; }
        public string? TransactionId { get; set; }
        public bool IsSuccessful { get; set; }
        public string? FailureReason { get; set; }
        public DateTime PaymentDate { get; set; }

        // Navigation Property
        public Reservation Reservation { get; set; } = null!;
    }
}