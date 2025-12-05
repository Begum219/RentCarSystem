using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class DamageReportDTO
    {
        public int Id { get; set; }

        // Reservation bilgileri
        public int ReservationId { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;

        // RentalAgreement bilgileri
        public int RentalAgreementId { get; set; }
        public string ContractNumber { get; set; } = string.Empty;

        // User & Vehicle bilgileri
        public string UserFullName { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;

        // Damage bilgileri
        public DateTime ReportDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public bool IsInsuranceCovered { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Resolution { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}