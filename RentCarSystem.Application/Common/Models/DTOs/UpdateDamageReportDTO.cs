using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class UpdateDamageReportDTO
    {
        public string? Description { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public bool? IsInsuranceCovered { get; set; }
        public string? Status { get; set; }
        public string? Resolution { get; set; }

    }
}
