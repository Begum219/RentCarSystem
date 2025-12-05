using System.ComponentModel.DataAnnotations;

namespace RentCarSystem.Application.Common.Models.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? TwoFactorCode { get; set; }
    }
}