using Microsoft.EntityFrameworkCore;
using RentCarSystem.Infrastructure.Context;
using Serilog;

namespace RentCarSystem.Application.FraudDetection
{
    public class FraudDetectionService : IFraudDetectionService
    {
        private readonly ApplicationDbContext _context;

        
        private static Dictionary<string, List<DateTime>> _failedLoginAttempts = new();

        public FraudDetectionService(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<bool> IsLoginBlocked(string email)
        {
            if (!_failedLoginAttempts.ContainsKey(email))
                return false;

            // Son 15 dakikadaki başarısız denemeler
            var recentAttempts = _failedLoginAttempts[email]
                .Where(d => d > DateTime.UtcNow.AddMinutes(-15))
                .ToList();

            // Eski denemeleri temizleme
            _failedLoginAttempts[email] = recentAttempts;

            if (recentAttempts.Count >= 5)
            {
                Log.Warning(" FRAUD ALERT: Login blocked for {Email} - {Attempts} failed attempts in 15 minutes",
                    email, recentAttempts.Count);
                return true;
            }

            return false;
        }

        public async Task RecordFailedLogin(string email)
        {
            if (!_failedLoginAttempts.ContainsKey(email))
                _failedLoginAttempts[email] = new List<DateTime>();

            _failedLoginAttempts[email].Add(DateTime.UtcNow);

            var count = _failedLoginAttempts[email]
                .Where(d => d > DateTime.UtcNow.AddMinutes(-15))
                .Count();

            Log.Warning("Failed login attempt #{Count}/5 for {Email}", count, email);

            await Task.CompletedTask;
        }

        public async Task RecordSuccessfulLogin(string email)
        {
            // Başarılı login sonrası temizle
            if (_failedLoginAttempts.ContainsKey(email))
            {
                _failedLoginAttempts.Remove(email);
                Log.Information(" Başarılı login for {Email} - Failed attempts cleared", email);
            }

            await Task.CompletedTask;
        }

  
        public async Task<int> CalculateReservationRiskScore(int userId, DateTime startDate, DateTime endDate, int vehicleId)
        {
            int riskScore = 0;
            var reasons = new List<string>();

            var user = await _context.Users.FindAsync(userId);
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);

            if (user == null || vehicle == null)
                return 0;

           
            var duration = endDate - startDate;
            if (duration.TotalHours < 1)
            {
                riskScore += 30;
                reasons.Add("Very short rental duration (< 1 hour)");
            }

           
            if (duration.TotalDays > 90)
            {
                riskScore += 20;
                reasons.Add("Very long rental duration (> 90 days)");
            }

            
            if (startDate.Hour >= 0 && startDate.Hour < 5)
            {
                riskScore += 15;
                reasons.Add("Late night reservation (00:00-05:00)");
            }

            
            var accountAge = (DateTime.UtcNow - user.CreatedAt).TotalDays;
            if (accountAge < 7 && vehicle.DailyPrice > 1000)
            {
                riskScore += 25;
                reasons.Add($"New user ({accountAge:F0} days old) + expensive vehicle (${vehicle.DailyPrice}/day)");
            }

           
            var activeReservations = await _context.Reservations
                .Where(r => r.UserId == userId &&
                           (r.Status == Domain.Enums.ReservationStatus.Pending ||
                            r.Status == Domain.Enums.ReservationStatus.Confirmed))
                .CountAsync();

            if (activeReservations > 3)
            {
                riskScore += 30;
                reasons.Add($"User has {activeReservations} active reservations");
            }

            // Kural 6: Kullanıcının geçmiş iptal oranı
            var totalReservations = await _context.Reservations
                .Where(r => r.UserId == userId)
                .CountAsync();

            if (totalReservations > 3)
            {
                var cancelledReservations = await _context.Reservations
                    .Where(r => r.UserId == userId &&
                               r.Status == Domain.Enums.ReservationStatus.Cancelled)
                    .CountAsync();

                var cancellationRate = (double)cancelledReservations / totalReservations;

                if (cancellationRate > 0.5)
                {
                    riskScore += 20;
                    reasons.Add($"High cancellation rate: {cancellationRate:P0}");
                }
            }

            // Loglama
            if (riskScore > 0)
            {
                var reasonsText = string.Join(", ", reasons);

                if (riskScore > 70)
                {
                    Log.Error(" HIGH RISK Reservation - User: {Email}, Vehicle: {VehicleId}, Risk Score: {RiskScore}, Reasons: {Reasons}",
                        user.Email, vehicleId, riskScore, reasonsText);
                }
                else if (riskScore > 40)
                {
                    Log.Warning(" MEDIUM RISK Reservation - User: {Email}, Vehicle: {VehicleId}, Risk Score: {RiskScore}, Reasons: {Reasons}",
                        user.Email, vehicleId, riskScore, reasonsText);
                }
                else
                {
                    Log.Information(" LOW RISK Reservation - User: {Email}, Vehicle: {VehicleId}, Risk Score: {RiskScore}",
                        user.Email, vehicleId, riskScore);
                }
            }

            return riskScore;
        }

        public async Task<bool> IsSuspiciousRegistration(string email, string phoneNumber)
        {
            // Aynı email veya telefon ile kayıtlı kullanıcı var mı?
            var duplicateCount = await _context.Users
                .Where(u => u.Email == email || u.PhoneNumber == phoneNumber)
                .CountAsync();

            if (duplicateCount > 0)
            {
                Log.Warning("FRAUD ALERT: Duplicate registration attempt - Email: {Email}, Phone: {Phone}",
                    email, phoneNumber);
                return true;
            }

            // Son 5 dakikada kaç kayıt oldu? (Bot koruması)
            var recentRegistrations = await _context.Users
                .Where(u => u.CreatedAt > DateTime.UtcNow.AddMinutes(-5))
                .CountAsync();

            if (recentRegistrations > 10)
            {
                Log.Warning("FRAUD ALERT: Mass registration detected - {Count} registrations in 5 minutes",
                    recentRegistrations);
                return true;
            }

            return false;
        }
    }
}