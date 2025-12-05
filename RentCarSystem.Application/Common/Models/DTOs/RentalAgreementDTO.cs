using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class RentalAgreementDTO
    {
        public int Id {  get; set; }
        public int ReservationId { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;

        // Agreement bilgileri
        public string ContractNumber { get; set; } = string.Empty;

        // Teslim Alma Bilgileri
        public int PickupKilometer { get; set; }
        public int FuelLevelAtPickup { get; set; }
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

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
