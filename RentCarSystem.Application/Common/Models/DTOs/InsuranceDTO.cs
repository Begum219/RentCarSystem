using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class InsuranceDTO
    {
        public int Id { get; set; }

        // Vehicle bilgileri
        public int VehicleId { get; set; }
        public string VehicleModel { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;

        // Insurance bilgileri
        public string PolicyNumber { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CoverageType { get; set; } = string.Empty;
        public decimal Premium { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
