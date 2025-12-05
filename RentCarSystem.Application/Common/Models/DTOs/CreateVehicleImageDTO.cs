using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class CreateVehicleImageDTO
    {
        public int VehicleId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMainImage { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
    }
}
