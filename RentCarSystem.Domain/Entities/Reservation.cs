using RentCarSystem.Domain.Common;
using RentCarSystem.Domain.Enums;

namespace RentCarSystem.Domain.Entities
{
    public class Reservation : BaseAuditableEntity
    {
        // İlişkiler
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public int PickupLocationId { get; set; }
        public int ReturnLocationId { get; set; }
        public Guid PublicId { get; set; } = Guid.NewGuid();

        // Tarih ve Saat
        public DateTime PickupDate { get; set; }
        public TimeSpan PickupTime { get; set; }
        public DateTime ReturnDate { get; set; }
        public TimeSpan ReturnTime { get; set; }

        // Hesaplamalar
        public int TotalDays { get; set; }
        public int TotalHours { get; set; }

        // Fiyatlandırma
        public decimal BasePrice { get; set; }
        public decimal ExtraServicesFee { get; set; }
        public decimal InsuranceFee { get; set; }
        public decimal ExtraKilometerFee { get; set; }
        public decimal FuelDifferenceFee { get; set; }
        public decimal LateFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }

        // Depozito
        public decimal DepositAmount { get; set; }
        public DepositStatus DepositStatus { get; set; } = DepositStatus.Pending;

        // Ekstra Hizmetler
        public bool HasBabySeat { get; set; }
        public bool HasGPS { get; set; }
        public bool HasAdditionalDriver { get; set; }

        // Durum
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        public string? CancellationReason { get; set; }
        public DateTime? CancellationDate { get; set; }

        public string? Notes { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
        public Vehicle Vehicle { get; set; } = null!;
        public Location PickupLocation { get; set; } = null!;
        public Location ReturnLocation { get; set; } = null!;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public RentalAgreement? RentalAgreement { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}