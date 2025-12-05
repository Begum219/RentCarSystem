using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.Application.Services
{
    public class DamageReportService : IDamageReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DamageReportService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<DamageReportDTO>> GetAllReportsAsync()
        {
            var reports = await _context.DamageReports
                .Include(dr => dr.Reservation)
                    .ThenInclude(r => r.User)
                .Include(dr => dr.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .Include(dr => dr.RentalAgreement)
                .ToListAsync();

            return _mapper.Map<List<DamageReportDTO>>(reports);
        }

        public async Task<DamageReportDTO?> GetReportByIdAsync(int id)
        {
            var report = await _context.DamageReports
                .Include(dr => dr.Reservation)
                    .ThenInclude(r => r.User)
                .Include(dr => dr.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .Include(dr => dr.RentalAgreement)
                .FirstOrDefaultAsync(dr => dr.Id == id);

            return _mapper.Map<DamageReportDTO>(report);
        }

        public async Task<List<DamageReportDTO>> GetReportsByReservationAsync(int reservationId)
        {
            var reports = await _context.DamageReports
                .Include(dr => dr.Reservation)
                    .ThenInclude(r => r.User)
                .Include(dr => dr.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .Include(dr => dr.RentalAgreement)
                .Where(dr => dr.ReservationId == reservationId)
                .ToListAsync();

            return _mapper.Map<List<DamageReportDTO>>(reports);
        }

        public async Task<List<DamageReportDTO>> GetReportsByAgreementAsync(int agreementId)
        {
            var reports = await _context.DamageReports
                .Include(dr => dr.Reservation)
                    .ThenInclude(r => r.User)
                .Include(dr => dr.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .Include(dr => dr.RentalAgreement)
                .Where(dr => dr.RentalAgreementId == agreementId)
                .ToListAsync();

            return _mapper.Map<List<DamageReportDTO>>(reports);
        }

        public async Task<List<DamageReportDTO>> GetReportsByVehicleAsync(int vehicleId)
        {
            var reports = await _context.DamageReports
                .Include(dr => dr.Reservation)
                    .ThenInclude(r => r.User)
                .Include(dr => dr.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .Include(dr => dr.RentalAgreement)
                .Where(dr => dr.Reservation.VehicleId == vehicleId)
                .ToListAsync();

            return _mapper.Map<List<DamageReportDTO>>(reports);
        }

        public async Task<List<DamageReportDTO>> GetReportsByStatusAsync(string status)
        {
            var reports = await _context.DamageReports
                .Include(dr => dr.Reservation)
                    .ThenInclude(r => r.User)
                .Include(dr => dr.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .Include(dr => dr.RentalAgreement)
                .Where(dr => dr.Status == status)
                .ToListAsync();

            return _mapper.Map<List<DamageReportDTO>>(reports);
        }

        public async Task<DamageReportDTO> CreateReportAsync(CreateDamageReportDTO dto)
        {
            // Reservation kontrolü
            var reservation = await _context.Reservations.FindAsync(dto.ReservationId);
            if (reservation == null)
                throw new Exception("Reservation not found");

            // RentalAgreement kontrolü
            var agreement = await _context.RentalAgreements.FindAsync(dto.RentalAgreementId);
            if (agreement == null)
                throw new Exception("RentalAgreement not found");

            var report = _mapper.Map<DamageReport>(dto);
            report.ReportDate = DateTime.UtcNow;
            report.Status = "Reported";

            _context.DamageReports.Add(report);
            await _context.SaveChangesAsync();

            return await GetReportByIdAsync(report.Id)
                ?? throw new Exception("Report creation failed");
        }

        public async Task<DamageReportDTO> UpdateReportAsync(int id, UpdateDamageReportDTO dto)
        {
            var report = await _context.DamageReports.FindAsync(id);

            if (report == null)
                throw new Exception($"Report with id {id} not found");

            if (!string.IsNullOrEmpty(dto.Description))
                report.Description = dto.Description;

            if (dto.EstimatedCost.HasValue)
                report.EstimatedCost = dto.EstimatedCost.Value;

            if (dto.ActualCost.HasValue)
                report.ActualCost = dto.ActualCost.Value;

            if (dto.IsInsuranceCovered.HasValue)
                report.IsInsuranceCovered = dto.IsInsuranceCovered.Value;

            if (!string.IsNullOrEmpty(dto.Status))
                report.Status = dto.Status;

            if (!string.IsNullOrEmpty(dto.Resolution))
                report.Resolution = dto.Resolution;

            await _context.SaveChangesAsync();

            return await GetReportByIdAsync(report.Id)
                ?? throw new Exception("Report update failed");
        }

        public async Task<bool> ResolveReportAsync(int id, string resolution, decimal actualCost)
        {
            var report = await _context.DamageReports.FindAsync(id);

            if (report == null)
                return false;

            report.Status = "Resolved";
            report.Resolution = resolution;
            report.ActualCost = actualCost;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteReportAsync(int id)
        {
            var report = await _context.DamageReports.FindAsync(id);

            if (report == null)
                return false;

            _context.DamageReports.Remove(report);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}