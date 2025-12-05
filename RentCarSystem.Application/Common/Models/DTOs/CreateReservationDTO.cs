using System.ComponentModel.DataAnnotations;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class CreateReservationDTO
    {
        public int? UserId { get; set; }
        [Required]
        public int VehicleId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int PickupLocationId { get; set; }
        [Required]
        public int ReturnLocationId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? Discount { get; set; }  
    }
}