using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IReservationService
    {
        // Query (Okuma)
        Task<List<ReservationDTO>> GetAllReservationsAsync();
        Task<ReservationDTO?> GetReservationByIdAsync(int id); 
        Task<List<ReservationDTO>> GetReservationsByUserAsync(int userId);
        Task<List<ReservationDTO>> GetReservationsByVehicleAsync(int vehicleId);
        Task<List<ReservationDTO>> GetActiveReservationsAsync(); 
        Task<List<ReservationDTO>> GetPendingReservationsAsync();

        // Command (Yazma)
        Task<ReservationDTO> CreateReservationAsync(CreateReservationDTO dto); 
        Task<ReservationDTO> UpdateReservationAsync(int id, UpdateReservationDTO dto);
        Task<bool> CancelReservationAsync(int id, string reason);
        Task<bool> CompleteReservationAsync(int id);
        Task<bool> DeleteReservationAsync(int id);
        Task<ReservationDTO?> GetReservationByPublicIdAsync(Guid publicId);
        Task<ReservationDTO> UpdateReservationByPublicIdAsync(Guid publicId, UpdateReservationDTO dto);
        Task<bool> CancelReservationByPublicIdAsync(Guid publicId, string reason);
        Task<bool> CompleteReservationByPublicIdAsync(Guid publicId);
        Task<bool> DeleteReservationByPublicIdAsync(Guid publicId);

    }
}