using RentCarSystem.Application.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IDamageReportService
    {
        Task<List<DamageReportDTO>> GetAllReportsAsync();
        Task<DamageReportDTO?> GetReportByIdAsync(int id);
        Task<List<DamageReportDTO>> GetReportsByReservationAsync(int reservationId);
        Task<List<DamageReportDTO>> GetReportsByAgreementAsync(int agreementId);
        Task<List<DamageReportDTO>> GetReportsByVehicleAsync(int vehicleId);
        Task<List<DamageReportDTO>> GetReportsByStatusAsync(string status);

        Task<DamageReportDTO> CreateReportAsync(CreateDamageReportDTO dto);
        Task<DamageReportDTO> UpdateReportAsync(int id, UpdateDamageReportDTO dto);
        Task<bool> ResolveReportAsync(int id, string resolution, decimal actualCost);
        Task<bool> DeleteReportAsync(int id);
    }
}
