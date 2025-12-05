using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReviewService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ReviewDTO>> GetAllReviewsAsync()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .ToListAsync();

            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public async Task<ReviewDTO?> GetReviewByIdAsync(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .FirstOrDefaultAsync(r => r.Id == id);

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<List<ReviewDTO>> GetReviewsByVehicleAsync(int vehicleId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .Where(r => r.VehicleId == vehicleId)
                .ToListAsync();

            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public async Task<List<ReviewDTO>> GetReviewsByUserAsync(int userId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public async Task<List<ReviewDTO>> GetApprovedReviewsAsync()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .Where(r => r.IsApproved)
                .ToListAsync();

            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public async Task<List<ReviewDTO>> GetPendingReviewsAsync()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .Where(r => !r.IsApproved)
                .ToListAsync();

            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public async Task<ReviewDTO> CreateReviewAsync(CreateReviewDTO dto)
        {
            // Reservation kontrolü
            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .FirstOrDefaultAsync(r => r.Id == dto.ReservationId);

            if (reservation == null)
                throw new Exception("Reservation not found");

            // Rating kontrolü (1-5)
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new Exception("Rating must be between 1 and 5");

            var review = new Review
            {
                UserId = reservation.UserId,
                VehicleId = reservation.VehicleId,
                ReservationId = dto.ReservationId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                IsApproved = false  // Default: onay bekliyor
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return await GetReviewByIdAsync(review.Id)
                ?? throw new Exception("Review creation failed");
        }

        public async Task<ReviewDTO> UpdateReviewAsync(int id, UpdateReviewDTO dto)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                throw new Exception($"Review with id {id} not found");

            // Rating kontrolü (1-5)
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new Exception("Rating must be between 1 and 5");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            await _context.SaveChangesAsync();

            return await GetReviewByIdAsync(review.Id)
                ?? throw new Exception("Review update failed");
        }

        public async Task<bool> ApproveReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return false;

            review.IsApproved = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}