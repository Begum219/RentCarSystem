using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IVehicleService
    {
        // Query Methods
        Task<List<VehicleDTO>> GetAllVehiclesAsync();  //hepsini getir read işlemi
        Task<VehicleDTO?> GetVehicleByIdAsync(int id);   // id ye göre 1 tane getir read işlemi
        Task<List<VehicleDTO>> GetAvailableVehiclesAsync();   //sadece müsait araçlar getir Read
        Task<List<VehicleDTO>> GetVehiclesByBrandAsync(int brandId);
        Task<List<VehicleDTO>> GetVehiclesByCategoryAsync(int categoryId);

        // Command Methods
        Task<VehicleDTO> CreateVehicleAsync(CreateVehicleDTO dto);  //Create
        Task<VehicleDTO> UpdateVehicleAsync(int id, CreateVehicleDTO dto);  //update
        Task<bool> DeleteVehicleAsync(int id); // delete kısmı
        Task<dynamic> SearchVehiclesAsync(string query);
        Task<PaginatedResult<VehicleDTO>> GetVehiclesPaginatedAsync(int pageNumber, int pageSize);
        Task<PaginatedResult<VehicleDTO>> GetFilteredVehiclesAsync(FilterVehiclesDTO filter);
    }
}