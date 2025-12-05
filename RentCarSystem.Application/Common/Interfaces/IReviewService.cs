using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewDTO>> GetAllReviewsAsync();
        Task<ReviewDTO?> GetReviewByIdAsync(int id);
        Task<List<ReviewDTO>> GetReviewsByVehicleAsync(int vehicleId);
        Task<List<ReviewDTO>> GetReviewsByUserAsync(int userId);
        Task<List<ReviewDTO>> GetApprovedReviewsAsync();
        Task<List<ReviewDTO>> GetPendingReviewsAsync();

        Task<ReviewDTO> CreateReviewAsync(CreateReviewDTO dto);
        Task<ReviewDTO> UpdateReviewAsync(int id, UpdateReviewDTO dto);
        Task<bool> ApproveReviewAsync(int id);
        Task<bool> DeleteReviewAsync(int id);
    }
}