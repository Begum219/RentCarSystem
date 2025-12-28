using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Infrastructure.Context;
using Serilog;
using System.Globalization;
using IyzicoOptions = Iyzipay.Options;

namespace RentCarSystem.Application.Services
{
    public class IyzicoPaymentGatewayService : IPaymentGatewayService
    {
        private readonly ApplicationDbContext _context;
        private readonly IyzicoOptions _iyzicoOptions;

        public IyzicoPaymentGatewayService(
            ApplicationDbContext context,
            IOptions<IyzicoSettings> settings)
        {
            _context = context;

            var iyzicoSettings = settings.Value;

            _iyzicoOptions = new IyzicoOptions
            {
                ApiKey = iyzicoSettings.ApiKey,
                SecretKey = iyzicoSettings.SecretKey,
                BaseUrl = iyzicoSettings.BaseUrl
            };

            Log.Information("Iyzico Gateway initialized");
        }

        public async Task<PaymentResult> ProcessSimplePaymentAsync(int userId, SimplePaymentRequest request)
        {
            try
            {
                // 1. Kullanıcıyı database'den al (İyzico zorunlu kıldığı için gerekli)
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                    throw new Exception("Kullanıcı bulunamadı");

                Log.Information("IYZICO: Payment starting - UserId: {UserId}, Amount: {Amount}₺", userId, request.Amount);

                // 2. İyzico ödeme isteği oluştur
                var paymentRequest = new CreatePaymentRequest
                {
                    Locale = Locale.TR.ToString(),
                    ConversationId = request.ReservationId.ToString(),
                    Price = request.Amount.ToString("0.00", CultureInfo.InvariantCulture),
                    PaidPrice = request.Amount.ToString("0.00", CultureInfo.InvariantCulture),
                    Currency = Currency.TRY.ToString(),
                    Installment = 1,
                    BasketId = $"RES{request.ReservationId}",
                    PaymentChannel = PaymentChannel.WEB.ToString(),
                    PaymentGroup = PaymentGroup.PRODUCT.ToString()
                };

                // 3. Test kartı bilgileri (Backend'de sabit)
                paymentRequest.PaymentCard = new PaymentCard
                {
                    CardHolderName = "Test User",
                    CardNumber = "5528790000000008",
                    ExpireMonth = "12",
                    ExpireYear = "30",
                    Cvc = "123",
                    RegisterCard = 0
                };

                // 4. Alıcı bilgileri (Database'den - İyzico zorunlu)
                paymentRequest.Buyer = new Buyer
                {
                    Id = userId.ToString(),
                    Name = user.FirstName,
                    Surname = user.LastName,
                    Email = user.Email,
                    IdentityNumber = "11111111111",
                    RegistrationAddress = "Test Adres",
                    City = "Istanbul",
                    Country = "Turkey",
                    Ip = "85.34.78.112"
                };

                // 5. Adres bilgileri (İyzico zorunlu)
                var address = new Address
                {
                    ContactName = $"{user.FirstName} {user.LastName}",
                    City = "Istanbul",
                    Country = "Turkey",
                    Description = "Test Adres"
                };
                paymentRequest.BillingAddress = address;
                paymentRequest.ShippingAddress = address;

                // 6. Sepet bilgileri (İyzico zorunlu)
                paymentRequest.BasketItems = new List<BasketItem>
                {
                    new BasketItem
                    {
                        Id = "1",
                        Name = "Arac Kiralama",
                        Category1 = "Kiralama",
                        ItemType = BasketItemType.PHYSICAL.ToString(),
                        Price = request.Amount.ToString("0.00", CultureInfo.InvariantCulture)
                    }
                };

                // 7. İyzico'ya gönder
                var payment = await Task.Run(() => Payment.Create(paymentRequest, _iyzicoOptions));

                // 8. Sonucu kontrol et
                if (payment.Status == "success")
                {
                    Log.Information("IYZICO: Payment successful - TransactionId: {TransactionId}", payment.PaymentId);

                    return new PaymentResult
                    {
                        Success = true,
                        TransactionId = payment.PaymentId ?? "UNKNOWN",
                        Message = "Ödeme başarılı",
                        ProcessedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    Log.Warning("IYZICO: Payment failed - Error: {Error}", payment.ErrorMessage);

                    return new PaymentResult
                    {
                        Success = false,
                        Message = payment.ErrorMessage ?? "Ödeme başarısız",
                        ErrorCode = payment.ErrorCode ?? "UNKNOWN",
                        ProcessedAt = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, " IYZICO: Payment error");

                return new PaymentResult
                {
                    Success = false,
                    Message = $"Ödeme hatası: {ex.Message}",
                    ErrorCode = "EXCEPTION"
                };
            }
        }

       
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            throw new NotImplementedException("Bu metod kullanılmıyor");
        }

        public Task<RefundResult> RefundPaymentAsync(string transactionId, decimal amount)
        {
            throw new NotImplementedException("Bu metod kullanılmıyor");
        }
    }
}