using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IMaintenanceService
    {
        Task<List<MaintenanceDTO>> GetAllMaintenancesAsync();
        Task<MaintenanceDTO?> GetMaintenanceByIdAsync(int id);
        Task<List<MaintenanceDTO>> GetMaintenancesByVehicleAsync(int vehicleId);
        Task<List<MaintenanceDTO>> GetActiveMaintenancesAsync();
        Task<List<MaintenanceDTO>> GetCompletedMaintenancesAsync();

        Task<MaintenanceDTO> CreateMaintenanceAsync(CreateMaintenanceDTO dto);
        Task<MaintenanceDTO> UpdateMaintenanceAsync(int id, CreateMaintenanceDTO dto);
        Task<bool> CompleteMaintenanceAsync(int id);
        Task<bool> DeleteMaintenanceAsync(int id);
    }
}