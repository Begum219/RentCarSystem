using RentCarSystem.Domain.Common;

namespace RentCarSystem.Domain.Entities
{
    public class DamageReport : BaseAuditableEntity
    {
        public int ReservationId { get; set; }
        public int RentalAgreementId { get; set; }
        public DateTime ReportDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public bool IsInsuranceCovered { get; set; }
        public string Status { get; set; } = "Reported"; // Reported, UnderReview, Resolved
        public string? Resolution { get; set; }

        // Navigation Properties
        public Reservation Reservation { get; set; } = null!;
        public RentalAgreement RentalAgreement { get; set; } = null!;
        public ICollection<DamageImage> DamageImages { get; set; } = new List<DamageImage>();
    }
}