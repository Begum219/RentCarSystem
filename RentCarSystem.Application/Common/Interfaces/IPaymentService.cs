using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IPaymentService
    {
        // Query (Okuma)
        Task<List<PaymentDTO>> GetAllPaymentsAsync();
        Task<PaymentDTO?> GetPaymentByIdAsync(int id);
        Task<List<PaymentDTO>> GetPaymentsByReservationAsync(int reservationId);
        Task<List<PaymentDTO>> GetPaymentsByUserAsync(int userId);

        // Command (Yazma)
        Task<PaymentDTO> CreatePaymentAsync(CreatePaymentDTO dto);
        Task<bool> DeletePaymentAsync(int id);
    }
}