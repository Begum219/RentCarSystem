using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class BrandDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;

        public string? LogoUrl { get; set; }
        public bool IsActive {  get; set; }
    }
}
