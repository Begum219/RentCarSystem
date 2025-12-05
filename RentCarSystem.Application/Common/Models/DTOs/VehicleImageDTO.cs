using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class VehicleImageDTO
    {
        public int Id { get; set; }

        // Vehicle bilgileri
        public int VehicleId { get; set; }
        public string VehicleModel { get; set; } = string.Empty;

        // Image bilgileri
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMainImage { get; set; }
        public int DisplayOrder { get; set; }
    }
}
