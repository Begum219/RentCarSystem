using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IVehicleImageService
    {
        Task<List<VehicleImageDTO>> GetAllImagesAsync();
        Task<VehicleImageDTO?> GetImageByIdAsync(int id);
        Task<List<VehicleImageDTO>> GetImagesByVehicleAsync(int vehicleId);
        Task<VehicleImageDTO?> GetMainImageAsync(int vehicleId);

        Task<VehicleImageDTO> CreateImageAsync(CreateVehicleImageDTO dto);
        Task<VehicleImageDTO> UpdateImageAsync(int id, CreateVehicleImageDTO dto);
        Task<bool> SetMainImageAsync(int imageId);
        Task<bool> DeleteImageAsync(int id);
    }
}