using RentCarSystem.Domain.Common;

namespace RentCarSystem.Domain.Entities
{
    public class Maintenance : BaseAuditableEntity
    {
        public int VehicleId { get; set; }
        public string MaintenanceType { get; set; } = string.Empty; // Periyodik, Onarım, Lastik, Boya
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Cost { get; set; }
        public string? Description { get; set; }
        public string? ServiceProvider { get; set; }
        public bool IsCompleted { get; set; }

        // Navigation Property
        public Vehicle Vehicle { get; set; } = null!;
    }
}