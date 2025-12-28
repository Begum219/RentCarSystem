namespace RentCarSystem.Application.Common.Models.DTOs
{
    /// <summary>
    /// Tam ödeme isteği (Kart bilgileri ile)
    /// </summary>
    public class PaymentRequest
    {
        public int ReservationId { get; set; }
        public decimal Amount { get; set; }

        // Kart bilgileri
        public string CardHolderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpireMonth { get; set; } = string.Empty;
        public string ExpireYear { get; set; } = string.Empty;
        public string Cvc { get; set; } = string.Empty;

        // Alıcı bilgileri
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerSurname { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string BuyerPhone { get; set; } = string.Empty;
        public string BuyerIdentityNumber { get; set; } = string.Empty;
        public string BuyerAddress { get; set; } = string.Empty;
        public string BuyerCity { get; set; } = string.Empty;
    }

    /// <summary>
    /// Basit ödeme isteği (Kart bilgisi yok - Backend sabit kart kullanır)
    /// </summary>
    public class SimplePaymentRequest
    {
        public int ReservationId { get; set; }
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// Ödeme sonucu
    /// </summary>
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// İade sonucu
    /// </summary>
    public class RefundResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}