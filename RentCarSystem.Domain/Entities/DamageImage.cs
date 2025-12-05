using RentCarSystem.Domain.Common;

namespace RentCarSystem.Domain.Entities
{
    public class DamageImage : BaseEntity
    {
        public int DamageReportId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        // Navigation Property
        public DamageReport DamageReport { get; set; } = null!;
    }
}