namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class ReservationDTO
    {
        public Guid PublicId { get; set; }

        // User bilgileri
        public int UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;  

        // Vehicle bilgileri
        public int VehicleId { get; set; }
        public string VehicleModel { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;  

        // Tarihler
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }

        // Lokasyonlar
        public int PickupLocationId { get; set; }
        public string PickupLocationName { get; set; } = string.Empty;
        public int ReturnLocationId { get; set; }
        public string ReturnLocationName { get; set; } = string.Empty;

        // Fiyat
        public decimal TotalPrice { get; set; }
        public decimal? ExtraCharges { get; set; }  // ✅ Ekle
        public decimal? Discount { get; set; }  // ✅ Ekle
        public decimal FinalPrice { get; set; }

        // Durum
        public string Status { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }  // ✅ Ekle

        public DateTime CreatedAt { get; set; }  // ✅ Ekle
    }
}