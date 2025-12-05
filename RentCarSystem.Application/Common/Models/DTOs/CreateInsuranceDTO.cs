using System.ComponentModel.DataAnnotations;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class CreateInsuranceDTO
    {
        [Required(ErrorMessage = "VehicleId is required")]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "PolicyNumber is required")]
        [StringLength(50, ErrorMessage = "PolicyNumber cannot exceed 50 characters")]
        public string PolicyNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Provider is required")]
        [StringLength(100, ErrorMessage = "Provider cannot exceed 100 characters")]
        public string Provider { get; set; } = string.Empty;

        [Required(ErrorMessage = "StartDate is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "CoverageType is required")]
        [StringLength(100, ErrorMessage = "CoverageType cannot exceed 100 characters")]
        public string CoverageType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Premium is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Premium must be positive")]
        public decimal Premium { get; set; }
    }
}