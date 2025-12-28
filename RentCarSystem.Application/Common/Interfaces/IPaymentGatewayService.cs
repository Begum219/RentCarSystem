using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IPaymentGatewayService
    {
       
        /// Tam ödeme işle (Kart bilgileri ile)
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);

        /// Basit ödeme işle (Kullanıcı kart bilgisi girmiyor) 
        Task<PaymentResult> ProcessSimplePaymentAsync(int userId, SimplePaymentRequest request);

        /// Ödeme iade et
        Task<RefundResult> RefundPaymentAsync(string transactionId, decimal amount);
    }
}