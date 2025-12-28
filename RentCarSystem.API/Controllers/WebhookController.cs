using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Infrastructure.Context;
using Serilog;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WebhookController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("iyzico")]
        public async Task<IActionResult> IyzicoWebhook([FromBody] IyzicoWebhookDTO? dto)
        {
            try
            {
                Log.Information(" WEBHOOK RECEIVED: {@dto}", dto);

                // PaymentId bir çok formatta gelebiliyor → normalize ediyoruz
                var paymentId =
                    dto?.PaymentId?.ToString() ??
                    dto?.IyziPaymentId?.ToString() ??
                    null;

                if (paymentId == null)
                {
                    Log.Warning("WEBHOOK: PaymentId bulunamadı");
                    return Ok(new { success = false, message = "PaymentId is missing" });
                }

                var eventType = dto?.IyziEventType?.ToLower() ?? "";
                var status = dto?.Status?.ToLower() ?? "";

                Log.Information(" WEBHOOK: eventType={EventType}, paymentId={PaymentId}, status={Status}",
                    eventType, paymentId, status);

                // REFUND işleme
                if (eventType.Contains("refund"))
                {
                    await HandleRefund(paymentId);
                }
                // Ödeme başarılı / yetkilendirme başarılı
                else if (eventType.Contains("payment") ||
                         eventType.Contains("auth") ||
                         status == "success")
                {
                    await HandleSuccessfulPayment(paymentId);
                }
                else
                {
                    Log.Warning(" WEBHOOK: Tanımsız eventType geldi → İşlenmedi");
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "WEBHOOK: Error occurred");
                return Ok(new { success = false, message = ex.Message });
            }
        }

        private async Task HandleRefund(string paymentId)
        {
            Log.Information(" REFUND PROCESSING → PaymentId: {PaymentId}", paymentId);

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == paymentId);

            if (payment != null)
            {
                payment.IsSuccessful = false;
                payment.FailureReason = "Webhook: Refund";
                payment.UpdatedAt = DateTime.UtcNow;
                payment.UpdatedBy = "Webhook";

                await _context.SaveChangesAsync();

                Log.Information("REFUND: Payment updated");
            }
            else
            {
                Log.Warning("REFUND: Payment not found");
            }
        }

        private async Task HandleSuccessfulPayment(string paymentId)
        {
            Log.Information("PAYMENT SUCCESS PROCESSING → PaymentId: {PaymentId}", paymentId);

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == paymentId);

            if (payment != null)
            {
                payment.IsSuccessful = true;
                payment.UpdatedAt = DateTime.UtcNow;
                payment.UpdatedBy = "Webhook";

                await _context.SaveChangesAsync();

                Log.Information("PAYMENT SUCCESS: Updated");
            }
            else
            {
                Log.Information(" PAYMENT SUCCESS: Payment bulunamadı (henüz kaydedilmemiş olabilir)");
            }
        }
    }
}
