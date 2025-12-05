using RentCarSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Domain.Entities
{
    public class User :  BaseAuditableEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; }= string.Empty;
        public string Email { get; set; }= string.Empty;
        public string PhoneNumber {  get; set; }= string.Empty; 
        public string PasswordHash { get; set; } = string.Empty;

        //sürücü bilgileri
        public string? DriverLicenseNumber {  get; set; }
        public DateTime? DriverLicenseIssueDate { get; set; }
        public DateTime? DateofBirth { get; set; }
        public string? IdentityNumber {  get; set; }

        //adres
        public string? Address {  get; set; }
        public string? City {  get; set; }

        // Acil Durum
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone {  get; set; }
        public string Role { get; set; } = "Customer"; // Admin,Customer
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; } = false;
        // 2FA Properties
        public bool TwoFactorEnabled { get; set; } = false;
        public string? TwoFactorSecret { get; set; }

        // Email Verification 
        public bool EmailVerified { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationExpiry { get; set; }

        // Password Reset 
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }

        // Refresh Token 
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        // Navigation Properties
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
