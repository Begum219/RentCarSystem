using RentCarSystem.Domain.Common;

namespace RentCarSystem.Domain.Entities
{
    public class Insurance : BaseAuditableEntity
    {
        public int VehicleId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CoverageType { get; set; } = string.Empty;
        public decimal Premium { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Property
        public Vehicle Vehicle { get; set; } = null!;
    }
}