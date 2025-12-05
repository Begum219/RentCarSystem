using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class RegisterUserDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; }= string.Empty;
        public string PasswordConfirm {  get; set; } = string.Empty;
        public string? DriverLicenseNumber {  get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
