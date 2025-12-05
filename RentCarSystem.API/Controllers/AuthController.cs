using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Application.Common.Models.DTOs.Auth;
using System.Security.Claims;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Yeni kullanıcı kaydı
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<TokenResponseDto>> Register([FromBody] RegisterUserDTO dto)
        {
            var response = await _authService.RegisterAsync(dto);
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcı girişi
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);
            return Ok(response);
        }

        /// <summary>
        /// Token yenileme
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var response = await _authService.RefreshTokenAsync(dto.RefreshToken);
            return Ok(response);
        }

        /// <summary>
        /// 2FA'yı etkinleştir (QR code döner)
        /// </summary>
        [HttpPost("enable-2fa")]
        [Authorize]
        public async Task<ActionResult<string>> Enable2FA()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var qrCode = await _authService.Enable2FAAsync(userId);
            return Ok(new { qrCode });
        }

        /// <summary>
        /// 2FA kodunu doğrula ve aktif et
        /// </summary>
        [HttpPost("verify-2fa")]
        [Authorize]
        public async Task<ActionResult> Verify2FA([FromBody] Enable2FADTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _authService.Verify2FAAsync(userId, dto.Code);
            return Ok(new { message = "2FA enabled successfully" });
        }

        /// <summary>
        /// 2FA'yı devre dışı bırak
        /// </summary>
        [HttpPost("disable-2fa")]
        [Authorize]
        public async Task<ActionResult> Disable2FA()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _authService.Disable2FAAsync(userId);
            return Ok(new { message = "2FA disabled successfully" });
        }

        /// <summary>
        /// Şifremi unuttum (email gönderir)
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            await _authService.ForgotPasswordAsync(dto.Email);
            return Ok(new { message = "Password reset email sent" });
        }

        /// <summary>
        /// Şifre sıfırla
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);
            return Ok(new { message = "Password reset successfully" });
        }

        /// <summary>
        /// Email doğrula
        /// </summary>
        [HttpGet("verify-email")]
        public async Task<ActionResult> VerifyEmail([FromQuery] string token)
        {
            await _authService.VerifyEmailAsync(token);
            return Ok(new { message = "Email verified successfully" });
        }
    }
}