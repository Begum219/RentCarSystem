using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class CreateReviewDTO
    {
        public int ReservationId { get; set; }
        public int Rating { get; set; }  // 1-5
        public string? Comment { get; set; }
    }
}
