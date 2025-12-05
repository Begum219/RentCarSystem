namespace RentCarSystem.Application.Common.Models.DTOs.Auth
{
    public class TokenResponseDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string? TwoFactorQrCode { get; set; }
        public bool RequiresTwoFactor { get; set; }
    }
}