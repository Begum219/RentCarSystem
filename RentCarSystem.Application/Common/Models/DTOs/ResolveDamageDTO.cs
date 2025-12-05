using System.ComponentModel.DataAnnotations;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class ResolveDamageDTO
    {
        [Required(ErrorMessage = "Resolution is required")]
        [StringLength(500, ErrorMessage = "Resolution cannot exceed 500 characters")]
        public string Resolution { get; set; } = string.Empty;

        [Required(ErrorMessage = "ActualCost is required")]
        [Range(0, double.MaxValue, ErrorMessage = "ActualCost must be positive")]
        public decimal ActualCost { get; set; }
    }
}