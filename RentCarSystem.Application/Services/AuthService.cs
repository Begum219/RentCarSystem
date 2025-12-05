using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Application.Common.Models.DTOs.Auth;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;
using System.Security.Cryptography;

namespace RentCarSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly ITwoFactorService _twoFactorService;
        //private readonly IEmailService _emailService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            ApplicationDbContext context,
            ITokenService tokenService,
            ITwoFactorService twoFactorService,
            //IEmailService emailService,
            IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _tokenService = tokenService;
            _twoFactorService = twoFactorService;
            //_emailService = emailService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<TokenResponseDto> RegisterAsync(RegisterUserDTO dto)
        {
            // Email kontrolü
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
                throw new Exception("Email already exists");

            // Şifre eşleşiyor mu?
            if (dto.Password != dto.PasswordConfirm)
                throw new Exception("Passwords do not match");

            // Email verification token oluştur
            var emailToken = GenerateRandomToken();

            // Yeni kullanıcı oluştur
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                DriverLicenseNumber = dto.DriverLicenseNumber,
                DateofBirth = dto.DateOfBirth,
                PasswordHash = HashPassword(dto.Password),
                Role = "Customer",
                IsActive = true,
                EmailVerified = false,
                EmailVerificationToken = emailToken,
                EmailVerificationExpiry = DateTime.UtcNow.AddDays(1)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Verification email gönder
            //await _emailService.SendVerificationEmailAsync(user.Email, emailToken);

            // Token oluştur
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Refresh token kaydet
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                RequiresTwoFactor = false
            };
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            // Kullanıcıyı bul
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);

            if (user == null)
                throw new Exception("Invalid email or password");

            // Şifre kontrolü
            if (!VerifyPassword(dto.Password, user.PasswordHash))
                throw new Exception("Invalid email or password");

            // 2FA kontrolü
            if (user.TwoFactorEnabled)
            {
                if (string.IsNullOrEmpty(dto.TwoFactorCode))
                {
                    // 2FA kodu gerekli
                    return new TokenResponseDto
                    {
                        RequiresTwoFactor = true
                    };
                }

                // 2FA kodunu doğrula
                if (!_twoFactorService.ValidateCode(user.TwoFactorSecret!, dto.TwoFactorCode))
                    throw new Exception("Invalid two-factor code");
            }

            // Token oluştur
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Refresh token kaydet
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                RequiresTwoFactor = false
            };
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.IsActive);

            if (user == null)
                throw new Exception("Invalid refresh token");

            // Refresh token süresi dolmuş mu?
            if (user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new Exception("Refresh token expired");

            // Yeni token'lar oluştur
            var accessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Refresh token güncelle
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                RequiresTwoFactor = false
            };
        }

        public async Task<string> Enable2FAAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            // Secret oluştur
            var secret = _twoFactorService.GenerateSecret();

            // QR code URI oluştur
            var qrCodeUri = _twoFactorService.GenerateQrCodeUri(user.Email, secret);

            // QR code oluştur
            var qrCodeBytes = _twoFactorService.GenerateQrCode(qrCodeUri);
            var qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);

            // Secret'ı kaydet (henüz aktif değil)
            user.TwoFactorSecret = secret;
            await _context.SaveChangesAsync();

            return $"data:image/png;base64,{qrCodeBase64}";
        }

        public async Task<bool> Verify2FAAsync(int userId, string code)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            if (string.IsNullOrEmpty(user.TwoFactorSecret))
                throw new Exception("2FA not initiated");

            // Kodu doğrula
            if (!_twoFactorService.ValidateCode(user.TwoFactorSecret, code))
                throw new Exception("Invalid code");

            // 2FA'yı aktif et
            user.TwoFactorEnabled = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Disable2FAAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.TwoFactorEnabled = false;
            user.TwoFactorSecret = null;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null)
                return true; 

            // Reset token oluştur
            var resetToken = GenerateRandomToken();

            user.PasswordResetToken = resetToken;
            user.PasswordResetExpiry = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            // Reset email gönder
            //await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token && u.IsActive);

            if (user == null)
                throw new Exception("Invalid or expired token");

            // Token süresi dolmuş mu?
            if (user.PasswordResetExpiry < DateTime.UtcNow)
                throw new Exception("Token expired");

            // Şifreyi güncelle
            user.PasswordHash = HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetExpiry = null;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailVerificationToken == token);

            if (user == null)
                throw new Exception("Invalid verification token");

            // Token süresi dolmuş mu?
            if (user.EmailVerificationExpiry < DateTime.UtcNow)
                throw new Exception("Verification token expired");

            // Email'i doğrula
            user.EmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationExpiry = null;
            await _context.SaveChangesAsync();

            return true;
        }

        // Helper Methods
        private string HashPassword(string password)
        {
           
            return password; // Geçici
        }

        private bool VerifyPassword(string password, string hash)
        {
            
            return password == hash; // Geçici
        }

        private string GenerateRandomToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}