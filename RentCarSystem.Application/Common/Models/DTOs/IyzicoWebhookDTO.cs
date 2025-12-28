namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class IyzicoWebhookDTO
    {
        public string? Status { get; set; }                // SUCCESS / FAILURE / REFUND
        public string? PaymentConversationId { get; set; } // Sipariş id (PaymentRequest.conversationId)
        public long? PaymentId { get; set; }               // Eski format paymentId
        public long? IyziPaymentId { get; set; }           // Yeni format
        public string? IyziReferenceCode { get; set; }
        public string? IyziEventType { get; set; }         // payment.succeeded, refund.failed vb.
        public long? IyziEventTime { get; set; }
        public int? MerchantId { get; set; }
    }
}
