using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Infrastructure.Context;
using Serilog;
using System.Security.Claims;

namespace RentCarSystem.Infrastructure.Services
{
    public class MockPaymentGatewayService : IPaymentGatewayService
    {
        private readonly ApplicationDbContext _context;

        // Test kartları ve sonuçları
        private readonly Dictionary<string, PaymentTestResult> _testCards = new()
        {
            // ✅ Başarılı kartlar
            { "4111111111111111", new PaymentTestResult { Success = true, Message = "Ödeme başarılı" } },
            { "5528790010000001", new PaymentTestResult { Success = true, Message = "Ödeme başarılı" } },
            { "5406670010000009", new PaymentTestResult { Success = true, Message = "Ödeme başarılı" } },
            
            // ❌ Başarısız kartlar
            { "4000000000000002", new PaymentTestResult { Success = false, Message = "Kart reddedildi", ErrorCode = "CARD_DECLINED" } },
            { "4000000000000069", new PaymentTestResult { Success = false, Message = "Yetersiz bakiye", ErrorCode = "INSUFFICIENT_FUNDS" } },
            { "4000000000000127", new PaymentTestResult { Success = false, Message = "CVC hatalı", ErrorCode = "INVALID_CVC" } },
            { "4000000000000119", new PaymentTestResult { Success = false, Message = "Kart süresi dolmuş", ErrorCode = "EXPIRED_CARD" } }
        };

        public MockPaymentGatewayService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Ödeme işle (Tam kart bilgileri ile)
        /// </summary>
        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            try
            {
                Log.Information("💳 MOCK: Processing payment - ReservationId: {ReservationId}, Amount: {Amount}₺, Card: ****{Last4}",
                    request.ReservationId,
                    request.Amount,
                    request.CardNumber.Length >= 4 ? request.CardNumber.Substring(request.CardNumber.Length - 4) : "****");

                // Ödeme işlemi simülasyonu (2 saniye bekle)
                await Task.Delay(2000);

                // Kart numarasını temizle (boşlukları ve tire işaretlerini kaldır)
                var cleanCardNumber = request.CardNumber.Replace(" ", "").Replace("-", "");

                // Test kartı kontrolü
                if (_testCards.TryGetValue(cleanCardNumber, out var testResult))
                {
                    if (testResult.Success)
                    {
                        var transactionId = $"MOCK-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

                        Log.Information("✅ MOCK: Payment successful - TransactionId: {TransactionId}, ReservationId: {ReservationId}",
                            transactionId, request.ReservationId);

                        return new PaymentResult
                        {
                            Success = true,
                            TransactionId = transactionId,
                            Message = testResult.Message,
                            ProcessedAt = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        Log.Warning("❌ MOCK: Payment failed - Error: {Error}, ErrorCode: {ErrorCode}",
                            testResult.Message, testResult.ErrorCode);

                        return new PaymentResult
                        {
                            Success = false,
                            TransactionId = string.Empty,
                            Message = testResult.Message,
                            ErrorCode = testResult.ErrorCode,
                            ProcessedAt = DateTime.UtcNow
                        };
                    }
                }

                // Tanınmayan kart
                Log.Warning(" MOCK: Unknown card - Card: ****{Last4}",
                    cleanCardNumber.Length >= 4 ? cleanCardNumber.Substring(cleanCardNumber.Length - 4) : "****");

                return new PaymentResult
                {
                    Success = false,
                    TransactionId = string.Empty,
                    Message = "Geçersiz kart numarası",
                    ErrorCode = "INVALID_CARD",
                    ProcessedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, " MOCK: Payment processing error - ReservationId: {ReservationId}", request.ReservationId);

                return new PaymentResult
                {
                    Success = false,
                    Message = $"Ödeme işlemi sırasında hata oluştu: {ex.Message}",
                    ErrorCode = "EXCEPTION"
                };
            }
        }

        /// <summary>
        /// Basit ödeme işle (Kullanıcı kart bilgisi girmiyor - Backend sabit test kartı kullanıyor)
        /// </summary>
        public async Task<PaymentResult> ProcessSimplePaymentAsync(int userId, SimplePaymentRequest request)
        {
            try
            {
                // Kullanıcı bilgilerini al
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    Log.Error(" MOCK: User not found - UserId: {UserId}", userId);
                    throw new Exception("Kullanıcı bulunamadı");
                }

                Log.Information(" MOCK: Simple payment processing - UserId: {UserId}, ReservationId: {ReservationId}, Amount: {Amount}₺",
                    userId, request.ReservationId, request.Amount);

                // Sabit test kartı kullan (Backend'de)
                var paymentRequest = new PaymentRequest
                {
                    ReservationId = request.ReservationId,
                    Amount = request.Amount,

                    // Sabit test kartı (Başarılı)
                    CardNumber = "4111111111111111",
                    CardHolderName = "Test User",
                    ExpireMonth = "12",
                    ExpireYear = "30",
                    Cvc = "123",

                    // Kullanıcı bilgileri
                    BuyerName = user.FirstName,
                    BuyerSurname = user.LastName,
                    BuyerEmail = user.Email,
                    BuyerPhone = user.PhoneNumber ?? "+905551234567",
                    BuyerIdentityNumber = "11111111111",
                    BuyerAddress = "Test Adres",
                    BuyerCity = "Istanbul"
                };

                // Normal ödeme metodunu çağır
                var result = await ProcessPaymentAsync(paymentRequest);

                Log.Information(" MOCK: Simple payment completed - Success: {Success}, UserId: {UserId}",
                    result.Success, userId);

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MOCK: Simple payment error - UserId: {UserId}, ReservationId: {ReservationId}",
                    userId, request.ReservationId);

                return new PaymentResult
                {
                    Success = false,
                    Message = $"Ödeme hatası: {ex.Message}",
                    ErrorCode = "EXCEPTION"
                };
            }
        }

        /// <summary>
        /// Ödeme iade et
        /// </summary>
        public async Task<RefundResult> RefundPaymentAsync(string transactionId, decimal amount)
        {
            try
            {
                Log.Information("💸 MOCK: Processing refund - TransactionId: {TransactionId}, Amount: {Amount}₺",
                    transactionId, amount);

                // İade işlemi simülasyonu (1 saniye bekle)
                await Task.Delay(1000);

                // Mock transaction ID kontrolü
                if (transactionId.StartsWith("MOCK-"))
                {
                    Log.Information("✅ MOCK: Refund successful - TransactionId: {TransactionId}", transactionId);

                    return new RefundResult
                    {
                        Success = true,
                        Message = "İade başarılı"
                    };
                }
                else
                {
                    Log.Warning("❌ MOCK: Invalid transaction ID - TransactionId: {TransactionId}", transactionId);

                    return new RefundResult
                    {
                        Success = false,
                        Message = "Geçersiz işlem numarası"
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ MOCK: Refund error - TransactionId: {TransactionId}", transactionId);

                return new RefundResult
                {
                    Success = false,
                    Message = $"İade işlemi sırasında hata oluştu: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Test sonucu modeli (Private)
        /// </summary>
        private class PaymentTestResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public string ErrorCode { get; set; } = string.Empty;
        }
    }
}
