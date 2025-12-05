using RentCarSystem.Application.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IInsuranceService
    {
        Task<List<InsuranceDTO>> GetAllInsurancesAsync();
        Task<InsuranceDTO?> GetInsuranceByIdAsync(int id);
        Task<List<InsuranceDTO>> GetInsurancesByVehicleAsync(int vehicleId);
        Task<List<InsuranceDTO>> GetActiveInsurancesAsync();
        Task<List<InsuranceDTO>> GetExpiringInsurancesAsync(int days);

        Task<InsuranceDTO> CreateInsuranceAsync(CreateInsuranceDTO dto);
        Task<InsuranceDTO> UpdateInsuranceAsync(int id, CreateInsuranceDTO dto);
        Task<bool> DeleteInsuranceAsync(int id);
    }
}
