using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Domain.Enums;
using RentCarSystem.Infrastructure.Context;

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

        public async Task<List<ReservationDTO>> GetAllReservationsAsync()
        {
            var reservations = await _context.Reservations 
                .Include(r => r.Vehicle)
                .Include(r => r.PickupLocation)
                .Include(r => r.ReturnLocation)
                .ToListAsync();

            return _mapper.Map<List<ReservationDTO>>(reservations);  
        }

        public async Task<ReservationDTO?> GetReservationByIdAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
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
                .Include(r => r.PickupLocation)
                .Include(r => r.ReturnLocation)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return _mapper.Map<List<ReservationDTO>>(reservations); 
        }

        public async Task<List<ReservationDTO>> GetReservationsByVehicleAsync(int vehicleId)
        {
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .Include(r => r.PickupLocation)
                .Include(r => r.ReturnLocation)
                .Where(r => r.VehicleId == vehicleId)
                .ToListAsync();

            return _mapper.Map<List<ReservationDTO>>(reservations);  
        }

        public async Task<List<ReservationDTO>> GetActiveReservationsAsync()
        {
            var now = DateTime.UtcNow;
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
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
                Status = ReservationStatus.Pending
            };

            _context.Reservations.Add(reservation);
            vehicle.Status = VehicleStatus.Rented;
            await _context.SaveChangesAsync();

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

            await _context.SaveChangesAsync();

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
            reservation.Vehicle.Status = VehicleStatus.Available;

            await _context.SaveChangesAsync();

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
            reservation.Vehicle.Status = VehicleStatus.Available;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                return false;

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}