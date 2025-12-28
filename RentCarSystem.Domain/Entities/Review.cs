using RentCarSystem.Domain.Common;

namespace RentCarSystem.Domain.Entities
{
    public class Review : BaseAuditableEntity
    {
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public int ReservationId { get; set; }
        
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
        public bool IsApproved { get; set; } = false;

        // Navigation Properties
        public User User { get; set; } = null!;
        public Vehicle Vehicle { get; set; } = null!; 
        public Reservation Reservation { get; set; } = null!;
    }
}