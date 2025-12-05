using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }

        // User bilgileri
        public int UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;

        // Vehicle bilgileri
        public int VehicleId { get; set; }
        public string VehicleModel { get; set; } = string.Empty;

        // Reservation bilgileri
        public int ReservationId { get; set; }

        // Review bilgileri
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsApproved { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}