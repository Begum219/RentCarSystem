namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class PaymentDTO
    {
        public Guid PublicId { get; set; }

        // Reservation bilgileri
        public int ReservationId { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;  // Entity'de yok, JOIN'den gelecek

        // Ödeme bilgileri
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;  // Enum → String
        public string PaymentType { get; set; } = string.Empty;    // Enum → String
        public string Status { get; set; } = string.Empty;         // Success, Failed, Pending
        public string? TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }

        // Kullanıcı bilgileri (JOIN'den gelecek)
        public int UserId { get; set; }  // Entity'de yok
        public string UserFullName { get; set; } = string.Empty;  // Entity'de yok
        public string UserEmail { get; set; } = string.Empty;

        // Araç bilgileri (JOIN'den gelecek)
        public int VehicleId { get; set; }  // Entity'de yok
        public string VehicleModel { get; set; } = string.Empty;  // Entity'de yok

        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}