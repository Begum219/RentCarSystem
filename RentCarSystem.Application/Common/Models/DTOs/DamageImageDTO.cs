using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class DamageImageDTO
    {
        public int Id { get; set; }

        // DamageReport bilgileri
        public int DamageReportId { get; set; }
        public string DamageDescription { get; set; } = string.Empty;

        // Image bilgileri
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}