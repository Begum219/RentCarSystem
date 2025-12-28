using RentCarSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Domain.Entities
{
    public class VehicleImage : BaseEntity

    {
        public int VehicleId { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMainImage { get; set; } = false;
        public int DisplayOrder { get; set; }

        // Navigation Property
        public Vehicle Vehicle { get; set; } = null!;

        

    }
}
