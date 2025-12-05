using System.ComponentModel.DataAnnotations;

namespace RentCarSystem.Application.Common.Models.DTOs.Auth
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}