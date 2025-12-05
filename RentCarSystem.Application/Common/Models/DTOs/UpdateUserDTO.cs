using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class UpdateUserDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [StringLength (200)]
        public string Email { get; set; } = string.Empty;
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        [StringLength(50)]
        public string? DriverLicenseNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

    }
}
