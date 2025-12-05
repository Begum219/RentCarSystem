namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class CreateRentalAgreementDTO
    {
        public int ReservationId { get; set; }

        // Teslim Alma Bilgileri
        public int PickupKilometer { get; set; }
        public int FuelLevelAtPickup { get; set; }
        public string? PickupNotes { get; set; }

        // İmza
        public string? CustomerSignatureUrl { get; set; }
        public string? StaffSignatureUrl { get; set; }
    }
}