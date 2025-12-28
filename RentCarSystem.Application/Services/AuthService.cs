using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Application.Common.Models.DTOs.Auth;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;
using System.Security.Cryptography;
using Serilog;
using RentCarSystem.Application.FraudDetection;
using RentCarSystem.Application.BackgroundJobs;
namespace RentCarSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly ITwoFactorService _twoFactorService;
        private readonly JwtSettings _jwtSettings;
        private readonly IFraudDetectionService _fraudDetection;
        private readonly IEmailService _emailService;  
        private readonly IBackgroundJobService _backgroundJobService; 
        public AuthService(
            ApplicationDbContext context,
            ITokenService tokenService,
            ITwoFactorService twoFactorService,
            IOptions<JwtSettings> jwtSettings,
             IFraudDetectionService fraudDetection,
             IEmailService emailService,
             IBackgroundJobService backgroundJobService)
        {
            _context = context;
            _tokenService = tokenService;
            _twoFactorService = twoFactorService;
            _jwtSettings = jwtSettings.Value;
            _fraudDetection = fraudDetection;
            _emailService = emailService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<TokenResponseDto> RegisterAsync(RegisterUserDTO dto)
        {
            Log.Information(" Registration attempt for email: {Email}", dto.Email);

            var isSuspicious = await _fraudDetection.IsSuspiciousRegistration(dto.Email, dto.PhoneNumber);
            if (isSuspicious)
            {
                Log.Warning("Registration blocked by fraud system for {Email}", dto.Email);
                throw new Exception("Suspicious registration detected.");
            }

            // Email kontrolü
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
            {
                Log.Warning(" Registration failed - Email already exists: {Email}", dto.Email);
                throw new Exception("Email already exists");
            }

            // Şifre eşleşiyor mu?
            if (dto.Password != dto.PasswordConfirm)
            {
                Log.Warning(" Registration failed - Passwords do not match for: {Email}", dto.Email);
                throw new Exception("Passwords do not match");
            }

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

            Log.Information("✅ User registered successfully: {Email}, Role: {Role}", user.Email, user.Role);

            //Background Job ile Email Gönderme
            _backgroundJobService.EnqueueVerificationEmail(user.Email, emailToken);
            _backgroundJobService.EnqueueWelcomeEmail(user.Email, $"{user.FirstName} {user.LastName}");

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
            Log.Information("Login attempt for email: {Email}", dto.Email);
            var isBlocked = await _fraudDetection.IsLoginBlocked(dto.Email);
            if (isBlocked)
            {
                Log.Warning(" Login blocked for {Email}", dto.Email);
                throw new BusinessException("Too many failed login attempts. Try again later.", 429);

            }
            // Kullanıcıyı bul
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);

            if (user == null)
            {
                Log.Warning(" Login failed - User not found or inactive: {Email}", dto.Email);
                throw new Exception("Invalid email or password");
            }

            // Şifre kontrolü
            if (!VerifyPassword(dto.Password, user.PasswordHash))
            {
                await _fraudDetection.RecordFailedLogin(dto.Email);

                Log.Warning(" Login failed - Invalid password for: {Email}", dto.Email);
                throw new Exception("Invalid email or password");
            }

            // Başarılı giriş → hataları temizle
            await _fraudDetection.RecordSuccessfulLogin(dto.Email);


            // 2FA kontrolü
            if (user.TwoFactorEnabled)
            {
                if (string.IsNullOrEmpty(dto.TwoFactorCode))
                {
                    Log.Information(" 2FA required for user: {Email}", dto.Email);
                    return new TokenResponseDto
                    {
                        RequiresTwoFactor = true
                    };
                }

                // 2FA kodunu doğrula
                if (!_twoFactorService.ValidateCode(user.TwoFactorSecret!, dto.TwoFactorCode))
                {
                    Log.Warning(" Login failed - Invalid 2FA code for: {Email}", dto.Email);
                    throw new Exception("Invalid two-factor code");
                }

                Log.Information(" 2FA validated for: {Email}", dto.Email);
            }

            // Token oluştur
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Refresh token kaydet
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            Log.Information("Login successful for: {Email}, Role: {Role}", user.Email, user.Role);

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
            Log.Information(" Refresh token attempt");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.IsActive);

            if (user == null)
            {
                Log.Warning(" Refresh token failed - Invalid token");
                throw new Exception("Invalid refresh token");
            }

            // Refresh token süresi dolmuş mu?
            if (user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                Log.Warning("Refresh token failed - Token expired for: {Email}", user.Email);
                throw new Exception("Refresh token expired");
            }

            // Yeni token'lar oluştur
            var accessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Refresh token güncelle
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            Log.Information(" Token refreshed for: {Email}", user.Email);

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
            Log.Information(" Enable 2FA attempt for userId: {UserId}", userId);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                Log.Warning(" Enable 2FA failed - User not found: {UserId}", userId);
                throw new Exception("User not found");
            }

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

            Log.Information(" 2FA QR code generated for: {Email}", user.Email);

            return $"data:image/png;base64,{qrCodeBase64}";
        }

        public async Task<bool> Verify2FAAsync(int userId, string code)
        {
            Log.Information(" Verify 2FA attempt for userId: {UserId}", userId);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                Log.Warning(" Verify 2FA failed - User not found: {UserId}", userId);
                throw new Exception("User not found");
            }

            if (string.IsNullOrEmpty(user.TwoFactorSecret))
            {
                Log.Warning("Verify 2FA failed - 2FA not initiated for: {Email}", user.Email);
                throw new Exception("2FA not initiated");
            }

            // Kodu doğrula
            if (!_twoFactorService.ValidateCode(user.TwoFactorSecret, code))
            {
                Log.Warning(" Verify 2FA failed - Invalid code for: {Email}", user.Email);
                throw new Exception("Invalid code");
            }

            // 2FA'yı aktif et
            user.TwoFactorEnabled = true;
            await _context.SaveChangesAsync();

            Log.Information(" 2FA enabled for: {Email}", user.Email);

            return true;
        }

        public async Task<bool> Disable2FAAsync(int userId)
        {
            Log.Information(" Disable 2FA attempt for userId: {UserId}", userId);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                Log.Warning(" Disable 2FA failed - User not found: {UserId}", userId);
                throw new Exception("User not found");
            }

            user.TwoFactorEnabled = false;
            user.TwoFactorSecret = null;
            await _context.SaveChangesAsync();

            Log.Information(" 2FA disabled for: {Email}", user.Email);

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            Log.Information(" Forgot password request for: {Email}", email);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null)
            {
                Log.Warning("Forgot password - User not found: {Email}", email);
                return true; // Güvenlik için başarılı gibi davran
            }

            // Reset token oluştur
            var resetToken = GenerateRandomToken();

            user.PasswordResetToken = resetToken;
            user.PasswordResetExpiry = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            Log.Information("Password reset token generated for: {Email}", user.Email);

            //Background Job ile Email Gönderme
            _backgroundJobService.EnqueuePasswordResetEmail(user.Email, resetToken);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            Log.Information(" Password reset attempt with token: {Token}", token);

            // Token ile kullanıcıyı bul
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token);

            if (user == null)
            {
                Log.Warning(" Password reset failed - Invalid token: {Token}", token);
                throw new Exception("Invalid or expired token");
            }

            Log.Information(" User found: {Email}", user.Email);

            // IsActive kontrolü
            if (!user.IsActive)
            {
                Log.Warning(" Password reset failed - User not active: {Email}", user.Email);
                throw new Exception("User is not active");
            }

            // Token süresi dolmuş mu?
            if (user.PasswordResetExpiry == null)
            {
                Log.Warning(" Password reset failed - Token expiry is null for: {Email}", user.Email);
                throw new Exception("Token expired");
            }

            if (user.PasswordResetExpiry < DateTime.UtcNow)
            {
                Log.Warning(" Password reset failed - Token expired for: {Email}. Expiry: {Expiry}, Now: {Now}",
                    user.Email, user.PasswordResetExpiry, DateTime.UtcNow);
                throw new Exception("Token expired");
            }

            // Şifreyi güncelle
            user.PasswordHash = HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetExpiry = null;

            await _context.SaveChangesAsync();

            Log.Information(" Password reset successful for: {Email}", user.Email);

            return true;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            Log.Information(" Email verification attempt with token: {Token}", token);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailVerificationToken == token);

            if (user == null)
            {
                Log.Warning(" Email verification failed - Invalid token: {Token}", token);
                throw new Exception("Invalid verification token");
            }

            // Token süresi dolmuş mu?
            if (user.EmailVerificationExpiry < DateTime.UtcNow)
            {
                Log.Warning(" Email verification failed - Token expired for: {Email}", user.Email);
                throw new Exception("Verification token expired");
            }

            // Email'i doğrula
            user.EmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationExpiry = null;
            await _context.SaveChangesAsync();

            Log.Information(" Email verified for: {Email}", user.Email);

            return true;
        }

        // Helper Methods
        private string HashPassword(string password)
        {
            // TODO: BCrypt ile hash'leme eklenecek
            return password; // Geçici
        }

        private bool VerifyPassword(string password, string hash)
        {
            // TODO: BCrypt ile doğrulama eklenecek
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