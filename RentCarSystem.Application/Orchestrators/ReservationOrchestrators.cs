using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Domain.Enums;
using RentCarSystem.Infrastructure.Context;
using Serilog;
using System.Text.Json;

using PaymentRequestEntity = RentCarSystem.Domain.Entities.PaymentRequest;

namespace RentCarSystem.Application.Orchestrators
{
    public class ReservationOrchestrator
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IDistributedCache _cache;

        public ReservationOrchestrator(
            ApplicationDbContext context,
            IPaymentGatewayService paymentGatewayService,
            IDistributedCache cache)
        {
            _context = context;
            _paymentGatewayService = paymentGatewayService;
            _cache = cache;
        }

        public async Task<ReservationWithPaymentResult> CreateReservationWithPaymentAsync(
            int userId,
            CreateReservationWithPaymentDTO dto)
        {
            var steps = new List<string>();
            string? errorStep = null;

            if (string.IsNullOrEmpty(dto.IdempotencyKey))
            {
                dto.IdempotencyKey = Guid.NewGuid().ToString();
                Log.Information("🔑 IDEMPOTENCY KEY: Auto-generated - Key: {Key}", dto.IdempotencyKey);
            }

            var cacheKey = $"idempotency:{dto.IdempotencyKey}";

            // STEP 0A: Redis cache kontrol
            try
            {
                var cachedJson = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedJson))
                {
                    var cachedResult = JsonSerializer.Deserialize<ReservationWithPaymentResult>(cachedJson);

                    if (cachedResult != null)
                    {
                        Log.Information("🔄 REDIS CACHE HIT: Key: {Key}", dto.IdempotencyKey);
                        return cachedResult;
                    }
                }

                Log.Information("⚡ REDIS CACHE MISS: Key: {Key}", dto.IdempotencyKey);
            }
            catch (Exception redisEx)
            {
                Log.Warning(redisEx, "⚠️ REDIS ERROR: Fallback to database");
            }

            // STEP 0B: Database kontrol
            var existingRequest = await _context.PaymentRequests
                .FirstOrDefaultAsync(p => p.IdempotencyKey == dto.IdempotencyKey);

            if (existingRequest != null)
            {
                var cachedResult = JsonSerializer.Deserialize<ReservationWithPaymentResult>(existingRequest.ResponseBody);

                if (cachedResult != null)
                {
                    Log.Information("🔄 DATABASE CACHE HIT: Key: {Key}", dto.IdempotencyKey);

                    try
                    {
                        await _cache.SetStringAsync(cacheKey, existingRequest.ResponseBody,
                            new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                            });
                    }
                    catch { }

                    return cachedResult;
                }
            }

            Log.Information("✅ IDEMPOTENCY KEY: New request - Key: {Key}", dto.IdempotencyKey);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Log.Information("🎯 ORCHESTRATOR START: UserId={UserId}, VehicleId={VehicleId}",
                    userId, dto.VehicleId);

                steps.Add("✅ Transaction started");

                var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == dto.VehicleId);

                if (vehicle == null)
                {
                    errorStep = "Vehicle not found";
                    throw new Exception($"Araç bulunamadı (Id: {dto.VehicleId})");
                }

                if (vehicle.Status != VehicleStatus.Available)
                {
                    errorStep = "Vehicle status check";
                    throw new Exception($"Araç müsait değil (Durum: {vehicle.Status})");
                }

                steps.Add($"✅ Vehicle found: {vehicle.Model}");
                Log.Information("✅ Vehicle validated");

                var existingReservationWithPayment = await _context.Reservations
                    .Where(r => r.UserId == userId
                                && r.VehicleId == dto.VehicleId
                                && r.PickupDate == dto.StartDate.Date
                                && r.ReturnDate == dto.EndDate.Date
                                && r.Status == ReservationStatus.Confirmed)
                    .Include(r => r.Payments)
                    .FirstOrDefaultAsync();

                if (existingReservationWithPayment?.Payments.Any(p => p.IsSuccessful) == true)
                {
                    errorStep = "Basic idempotency check";
                    Log.Warning("⚠️ BASIC IDEMPOTENCY: Duplicate payment blocked");
                    throw new Exception("Bu rezervasyon için ödeme zaten yapılmış");
                }

                steps.Add("✅ Basic idempotency check passed");

                var totalDays = (dto.EndDate - dto.StartDate).Days;

                var reservation = new Reservation
                {
                    UserId = userId,
                    VehicleId = dto.VehicleId,
                    PickupLocationId = dto.PickupLocationId,
                    ReturnLocationId = dto.ReturnLocationId,
                    PickupDate = dto.StartDate.Date,
                    PickupTime = dto.StartDate.TimeOfDay,
                    ReturnDate = dto.EndDate.Date,
                    ReturnTime = dto.EndDate.TimeOfDay,
                    TotalDays = totalDays,
                    TotalHours = totalDays * 24,
                    BasePrice = dto.PaymentAmount,
                    DiscountAmount = dto.Discount,
                    TotalPrice = dto.PaymentAmount - dto.Discount,
                    DepositAmount = vehicle.DepositAmount,
                    DepositStatus = DepositStatus.Pending,
                    Status = ReservationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                await _context.Reservations.AddAsync(reservation);
                await _context.SaveChangesAsync();

                steps.Add($"✅ Reservation created (Id: {reservation.Id})");

                var paymentRequest = new SimplePaymentRequest
                {
                    ReservationId = reservation.Id,
                    Amount = dto.PaymentAmount
                };

                var paymentResult = await _paymentGatewayService.ProcessSimplePaymentAsync(userId, paymentRequest);

                if (!paymentResult.Success)
                {
                    errorStep = "Payment processing";
                    throw new Exception($"Ödeme başarısız: {paymentResult.Message}");
                }

                steps.Add($"✅ Payment successful (TransactionId: {paymentResult.TransactionId})");

                var payment = new Payment
                {
                    ReservationId = reservation.Id,
                    Amount = dto.PaymentAmount,
                    TransactionId = paymentResult.TransactionId ?? "UNKNOWN",
                    IsSuccessful = true,
                    PaymentMethod = PaymentMethod.CreditCard,
                    PaymentType = PaymentType.Reservation,
                    PaymentDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

                steps.Add("✅ Payment saved");

                reservation.Status = ReservationStatus.Confirmed;
                reservation.UpdatedAt = DateTime.UtcNow;
                reservation.UpdatedBy = "System";
                await _context.SaveChangesAsync();

                steps.Add("✅ Reservation confirmed");

                vehicle.Status = VehicleStatus.Rented;
                vehicle.UpdatedAt = DateTime.UtcNow;
                vehicle.UpdatedBy = "System";
                await _context.SaveChangesAsync();

                steps.Add("✅ Vehicle updated");

                await transaction.CommitAsync();
                steps.Add("✅ Transaction committed");

                Log.Information("🎉 ORCHESTRATOR SUCCESS");

                var result = new ReservationWithPaymentResult
                {
                    Success = true,
                    ReservationId = reservation.Id,
                    TransactionId = paymentResult.TransactionId,
                    Message = "Rezervasyon ve ödeme başarıyla tamamlandı",
                    Steps = steps,
                    ErrorStep = null
                };

                var resultJson = JsonSerializer.Serialize(result);

                try
                {
                    var paymentRequestRecord = new PaymentRequestEntity
                    {
                        IdempotencyKey = dto.IdempotencyKey,
                        UserId = userId,
                        RequestBody = JsonSerializer.Serialize(dto),
                        ResponseBody = resultJson,
                        IsSuccessful = true,
                        ReservationId = reservation.Id,
                        TransactionId = paymentResult.TransactionId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    };

                    await _context.PaymentRequests.AddAsync(paymentRequestRecord);
                    await _context.SaveChangesAsync();

                    Log.Information("✅ DATABASE: Request saved");
                }
                catch (Exception saveEx)
                {
                    Log.Error(saveEx, "❌ DATABASE: Save failed");
                }

                try
                {
                    await _cache.SetStringAsync(cacheKey, resultJson,
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                        });

                    Log.Information("✅ REDIS: Cached for 24h");
                }
                catch (Exception redisEx)
                {
                    Log.Warning(redisEx, "⚠️ REDIS: Cache failed");
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ ORCHESTRATOR FAILED");

                await transaction.RollbackAsync();
                steps.Add("🔄 Transaction rolled back");

                var errorResult = new ReservationWithPaymentResult
                {
                    Success = false,
                    ReservationId = null,
                    TransactionId = null,
                    Message = $"İşlem başarısız: {ex.Message}",
                    Steps = steps,
                    ErrorStep = errorStep
                };

                var errorJson = JsonSerializer.Serialize(errorResult);

                try
                {
                    var paymentRequestRecord = new PaymentRequestEntity
                    {
                        IdempotencyKey = dto.IdempotencyKey,
                        UserId = userId,
                        RequestBody = JsonSerializer.Serialize(dto),
                        ResponseBody = errorJson,
                        IsSuccessful = false,
                        ReservationId = null,
                        TransactionId = null,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    };

                    await _context.PaymentRequests.AddAsync(paymentRequestRecord);
                    await _context.SaveChangesAsync();
                }
                catch { }

                try
                {
                    await _cache.SetStringAsync(cacheKey, errorJson,
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                        });
                }
                catch { }

                return errorResult;
            }
        }
    }

    public class ReservationWithPaymentResult
    {
        public bool Success { get; set; }
        public int? ReservationId { get; set; }
        public string? TransactionId { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Steps { get; set; } = new();
        public string? ErrorStep { get; set; }
    }
}