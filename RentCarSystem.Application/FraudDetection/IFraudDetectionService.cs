using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.FraudDetection
{
    public interface IFraudDetectionService
    {
        Task<bool> IsLoginBlocked(string email);
        Task RecordFailedLogin(string email);
        Task RecordSuccessfulLogin(string email);

        //Şüpheli rezrvasyon
        Task<int> CalculateReservationRiskScore(int userId, DateTime startDate, DateTime endDate, int vehicleId);
        //sahte hesap
        Task<bool> IsSuspiciousRegistration(string email, string phoneNumber);

    }
}
