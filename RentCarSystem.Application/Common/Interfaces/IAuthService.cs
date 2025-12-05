using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Application.Common.Models.DTOs.Auth;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto> RegisterAsync(RegisterUserDTO dto);
        Task<TokenResponseDto> LoginAsync(LoginDto dto);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
        Task<string> Enable2FAAsync(int userId);
        Task<bool> Verify2FAAsync(int userId, string code);
        Task<bool> Disable2FAAsync(int userId);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        Task<bool> VerifyEmailAsync(string token);
    }
}