using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Domain.Enums;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PaymentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PaymentDTO>> GetAllPaymentsAsync()
        {
            var payments = await _context.Payments
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.User)
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .ToListAsync();

            return _mapper.Map<List<PaymentDTO>>(payments); 
        }

        public async Task<PaymentDTO?> GetPaymentByIdAsync(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.User)
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<PaymentDTO>(payment);  
        }

        public async Task<List<PaymentDTO>> GetPaymentsByReservationAsync(int reservationId)
        {
            var payments = await _context.Payments
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.User)
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .Where(p => p.ReservationId == reservationId)
                .ToListAsync();

            return _mapper.Map<List<PaymentDTO>>(payments); 
        }

        public async Task<List<PaymentDTO>> GetPaymentsByUserAsync(int userId)
        {
            var payments = await _context.Payments
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.User)
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .Where(p => p.Reservation.UserId == userId)
                .ToListAsync();

            return _mapper.Map<List<PaymentDTO>>(payments);
        }

        public async Task<PaymentDTO> CreatePaymentAsync(CreatePaymentDTO dto)
        {
            // Reservation kontrolü
            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .FirstOrDefaultAsync(r => r.Id == dto.ReservationId);

            if (reservation == null)
                throw new Exception("Reservation not found");

            var payment = _mapper.Map<Payment>(dto);  
            payment.PaymentDate = DateTime.UtcNow;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return await GetPaymentByIdAsync(payment.Id)
                ?? throw new Exception("Payment creation failed");
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
                return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}