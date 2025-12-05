using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class UpdateReviewDTO
    {
        public int Rating { get; set; }  // 1-5
        public string? Comment { get; set; }
    }
}