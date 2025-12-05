using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class CreateDamageReportDTO
    {
        public int ReservationId { get; set; }
        public int RentalAgreementId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
        public bool IsInsuranceCovered { get; set; }
    }
}
