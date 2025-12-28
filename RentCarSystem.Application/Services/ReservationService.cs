using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Domain.Enums;
using RentCarSystem.Infrastructure.Context;
using Serilog;

namespace RentCarSystem.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReservationService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ========== INT ID İLE METODLAR (Mevcut) ==========

        public async Task<List<ReservationDTO>> GetAllReservationsAsync()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(r => r.PickupLocation)
                .Include(r => r.ReturnLocation)
                .Include(r => r.User)
                .ToListAsync();

            return _mapper.Map<List<ReservationDTO>>(reservations);
        }

        public async Task<ReservationDTO?> GetReservationByIdAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(r => r.PickupLocation)
                .Include(r => r.ReturnLocation)
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<ReservationDTO>(reservation);
        }

        public async Task<List<ReservationDTO>> GetReservationsByUserAsync(int userId)
        {
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(r => r.PickupLocation)
                .Include(r => r.ReturnLocation)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<ReservationDTO>>(reservations);
        }

        public async Task<List<ReservationDTO>> GetReservationsByVehicleAsync(int vehicleId)
        {
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(r => r.PickupLocation)
                .Include(r => r.ReturnLocation)
                .Where(r => r.VehicleId == vehicleId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<ReservationDTO>>(reservations);
        }

        public async Task<List<ReservationDTO>> GetActiveReservationsAsync()
        {
            var now = DateTime.UtcNow;
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(r => r.PickupLocation)
                .Include(r => r.ReturnLocation)
                .Where(r => r.Status == ReservationStatus.Active &&
                           r.PickupDate <= now && r.ReturnDate >= now)
                .ToListAsync();

            return _mapper.Map<List<ReservationDTO>>(reservations);
        }

        public async Task<List<ReservationDTO>> GetPendingReservationsAsync()
        {
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(r => r.PickupLocation)
                .Include(r => r.ReturnLocation)
                .Where(r => r.Status == ReservationStatus.Pending)
                .ToListAsync();

            return _mapper.Map<List<ReservationDTO>>(reservations);
        }

        public async Task<ReservationDTO> CreateReservationAsync(CreateReservationDTO dto)
        {
            // Araç kontrolü
            var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId);
            if (vehicle == null)
                throw new Exception("Vehicle not found");

            if (vehicle.Status != VehicleStatus.Available)
                throw new Exception("Vehicle is not available");

            // Gün hesabı
            var totalDays = (dto.EndDate - dto.StartDate).Days;
            var basePrice = vehicle.DailyPrice * totalDays;
            var discountAmount = dto.Discount ?? 0;
            var totalPrice = basePrice - discountAmount;

            var reservation = new Reservation
            {
                PublicId = Guid.NewGuid(),
                UserId = dto.UserId ?? 1,
                VehicleId = dto.VehicleId,
                PickupDate = dto.StartDate,
                PickupTime = TimeSpan.Zero,
                ReturnDate = dto.EndDate,
                ReturnTime = TimeSpan.Zero,
                PickupLocationId = dto.PickupLocationId,
                ReturnLocationId = dto.ReturnLocationId,
                TotalDays = totalDays,
                TotalHours = totalDays * 24,
                BasePrice = basePrice,
                ExtraServicesFee = 0,
                InsuranceFee = 0,
                ExtraKilometerFee = 0,
                FuelDifferenceFee = 0,
                LateFee = 0,
                DiscountAmount = discountAmount,
                TotalPrice = totalPrice,
                DepositAmount = vehicle.DepositAmount,
                DepositStatus = DepositStatus.Pending,
                HasBabySeat = false,
                HasGPS = false,
                HasAdditionalDriver = false,
                Status = ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.Reservations.Add(reservation);
            vehicle.Status = VehicleStatus.Rented;
            await _context.SaveChangesAsync();

            Log.Information("Reservation created: {PublicId}", reservation.PublicId);

            return await GetReservationByIdAsync(reservation.Id)
                ?? throw new Exception("Reservation creation failed");
        }

        public async Task<ReservationDTO> UpdateReservationAsync(int id, UpdateReservationDTO dto)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                throw new Exception($"Reservation with id {id} not found");

            if (dto.Status.HasValue)
                reservation.Status = (ReservationStatus)dto.Status.Value;

            if (dto.ExtraCharges.HasValue)
            {
                reservation.ExtraServicesFee = dto.ExtraCharges.Value;
                reservation.TotalPrice = reservation.BasePrice +
                                        reservation.ExtraServicesFee +
                                        reservation.InsuranceFee +
                                        reservation.ExtraKilometerFee +
                                        reservation.FuelDifferenceFee +
                                        reservation.LateFee -
                                        reservation.DiscountAmount;
            }

            reservation.UpdatedAt = DateTime.UtcNow;
            reservation.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            Log.Information("Reservation updated: {Id}, PublicId: {PublicId}", reservation.Id, reservation.PublicId);

            return await GetReservationByIdAsync(reservation.Id)
                ?? throw new Exception("Reservation update failed");
        }

        public async Task<bool> CancelReservationAsync(int id, string reason)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Vehicle)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return false;

            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancellationReason = reason;
            reservation.CancellationDate = DateTime.UtcNow;
            reservation.UpdatedAt = DateTime.UtcNow;
            reservation.UpdatedBy = "System";

            if (reservation.Vehicle != null)
            {
                reservation.Vehicle.Status = VehicleStatus.Available;
            }

            await _context.SaveChangesAsync();

            Log.Information("Reservation cancelled: {Id}, PublicId: {PublicId}, Reason: {Reason}",
                reservation.Id, reservation.PublicId, reason);

            return true;
        }

        public async Task<bool> CompleteReservationAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Vehicle)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return false;

            reservation.Status = ReservationStatus.Completed;
            reservation.UpdatedAt = DateTime.UtcNow;
            reservation.UpdatedBy = "System";

            if (reservation.Vehicle != null)
            {
                reservation.Vehicle.Status = VehicleStatus.Available;
            }

            await _context.SaveChangesAsync();

            Log.Information("Reservation completed: {Id}, PublicId: {PublicId}",
                reservation.Id, reservation.PublicId);

            return true;
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                return false;

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            Log.Information("Reservation deleted: {Id}", id);

            return true;
        }

        // ========== GUID PUBLICID İLE METODLAR (YENİ - EKLENDİ!) ==========

        public async Task<ReservationDTO?> GetReservationByPublicIdAsync(Guid publicId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(r => r.PickupLocation)
                .Include(r => r.ReturnLocation)
                .FirstOrDefaultAsync(r => r.PublicId == publicId);

            return _mapper.Map<ReservationDTO>(reservation);
        }

        public async Task<ReservationDTO> UpdateReservationByPublicIdAsync(Guid publicId, UpdateReservationDTO dto)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.PublicId == publicId);

            if (reservation == null)
                throw new Exception($"Reservation with PublicId {publicId} not found");

            if (dto.Status.HasValue)
                reservation.Status = (ReservationStatus)dto.Status.Value;

            if (dto.ExtraCharges.HasValue)
            {
                reservation.ExtraServicesFee = dto.ExtraCharges.Value;
                reservation.TotalPrice = reservation.BasePrice +
                                        reservation.ExtraServicesFee +
                                        reservation.InsuranceFee +
                                        reservation.ExtraKilometerFee +
                                        reservation.FuelDifferenceFee +
                                        reservation.LateFee -
                                        reservation.DiscountAmount;
            }

            reservation.UpdatedAt = DateTime.UtcNow;
            reservation.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            Log.Information("Reservation updated by PublicId: {PublicId}", publicId);

            return await GetReservationByPublicIdAsync(publicId)
                ?? throw new Exception("Reservation update failed");
        }

        public async Task<bool> CancelReservationByPublicIdAsync(Guid publicId, string reason)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Vehicle)
                .FirstOrDefaultAsync(r => r.PublicId == publicId);

            if (reservation == null)
                return false;

            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancellationReason = reason;
            reservation.CancellationDate = DateTime.UtcNow;
            reservation.UpdatedAt = DateTime.UtcNow;
            reservation.UpdatedBy = "System";

            if (reservation.Vehicle != null)
            {
                reservation.Vehicle.Status = VehicleStatus.Available;
            }

            await _context.SaveChangesAsync();

            Log.Information("Reservation cancelled by PublicId: {PublicId}, Reason: {Reason}",
                publicId, reason);

            return true;
        }

        public async Task<bool> CompleteReservationByPublicIdAsync(Guid publicId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Vehicle)
                .FirstOrDefaultAsync(r => r.PublicId == publicId);

            if (reservation == null)
                return false;

            reservation.Status = ReservationStatus.Completed;
            reservation.UpdatedAt = DateTime.UtcNow;
            reservation.UpdatedBy = "System";

            if (reservation.Vehicle != null)
            {
                reservation.Vehicle.Status = VehicleStatus.Available;
            }

            await _context.SaveChangesAsync();

            Log.Information("Reservation completed by PublicId: {PublicId}", publicId);

            return true;
        }

        public async Task<bool> DeleteReservationByPublicIdAsync(Guid publicId)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.PublicId == publicId);

            if (reservation == null)
                return false;

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            Log.Information("Reservation deleted by PublicId: {PublicId}", publicId);

            return true;
        }
    }
}