using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class CreateBrandDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name {  get; set; }=string.Empty;
        [Url]
        [StringLength(500)]
        public string? LogoUrl {  get; set; }  
    }
}
