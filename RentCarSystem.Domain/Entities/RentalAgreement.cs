using RentCarSystem.Domain.Common;

namespace RentCarSystem.Domain.Entities
{
    public class RentalAgreement : BaseAuditableEntity
    {
        public int ReservationId { get; set; }
        public string ContractNumber { get; set; } = string.Empty;

        // Teslim Alma Bilgileri
        public int PickupKilometer { get; set; }
        public int FuelLevelAtPickup { get; set; } // Yüzde olarak
        public string? PickupNotes { get; set; }
        public DateTime? PickupDateTime { get; set; }

        // İade Bilgileri
        public int? ReturnKilometer { get; set; }
        public int? FuelLevelAtReturn { get; set; }
        public string? ReturnNotes { get; set; }
        public DateTime? ReturnDateTime { get; set; }

        // İmza
        public string? CustomerSignatureUrl { get; set; }
        public string? StaffSignatureUrl { get; set; }

        // Navigation Property
        public Reservation Reservation { get; set; } = null!;
        public ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();
    }
}