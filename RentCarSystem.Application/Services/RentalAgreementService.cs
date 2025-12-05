using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.Application.Services
{
    public class RentalAgreementService : IRentalAgreementService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RentalAgreementService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RentalAgreementDTO>> GetAllAgreementsAsync()
        {
            var agreements = await _context.RentalAgreements
                .Include(ra => ra.Reservation)
                    .ThenInclude(r => r.User)
                .Include(ra => ra.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .ToListAsync();

            return _mapper.Map<List<RentalAgreementDTO>>(agreements);
        }

        public async Task<RentalAgreementDTO?> GetAgreementByIdAsync(int id)
        {
            var agreement = await _context.RentalAgreements
                .Include(ra => ra.Reservation)
                    .ThenInclude(r => r.User)
                .Include(ra => ra.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .FirstOrDefaultAsync(ra => ra.Id == id);

            return _mapper.Map<RentalAgreementDTO>(agreement);
        }

        public async Task<RentalAgreementDTO?> GetAgreementByReservationAsync(int reservationId)
        {
            var agreement = await _context.RentalAgreements
                .Include(ra => ra.Reservation)
                    .ThenInclude(r => r.User)
                .Include(ra => ra.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .FirstOrDefaultAsync(ra => ra.ReservationId == reservationId);

            return _mapper.Map<RentalAgreementDTO>(agreement);
        }

        public async Task<List<RentalAgreementDTO>> GetActiveAgreementsAsync()
        {
            var agreements = await _context.RentalAgreements
                .Include(ra => ra.Reservation)
                    .ThenInclude(r => r.User)
                .Include(ra => ra.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .Where(ra => ra.PickupDateTime != null && ra.ReturnDateTime == null)
                .ToListAsync();

            return _mapper.Map<List<RentalAgreementDTO>>(agreements);
        }

        public async Task<List<RentalAgreementDTO>> GetCompletedAgreementsAsync()
        {
            var agreements = await _context.RentalAgreements
                .Include(ra => ra.Reservation)
                    .ThenInclude(r => r.User)
                .Include(ra => ra.Reservation)
                    .ThenInclude(r => r.Vehicle)
                .Where(ra => ra.ReturnDateTime != null)
                .ToListAsync();

            return _mapper.Map<List<RentalAgreementDTO>>(agreements);
        }

        public async Task<RentalAgreementDTO> CreateAgreementAsync(CreateRentalAgreementDTO dto)
        {
            // Reservation kontrolü
            var reservation = await _context.Reservations.FindAsync(dto.ReservationId);
            if (reservation == null)
                throw new Exception("Reservation not found");

            // Zaten agreement var mı kontrol et
            var existingAgreement = await _context.RentalAgreements
                .FirstOrDefaultAsync(ra => ra.ReservationId == dto.ReservationId);

            if (existingAgreement != null)
                throw new Exception("Agreement already exists for this reservation");

            var agreement = _mapper.Map<RentalAgreement>(dto);

            // Otomatik alanlar
            agreement.ContractNumber = $"RNT-{DateTime.UtcNow:yyyyMMdd}-{dto.ReservationId:D6}";
            agreement.PickupDateTime = DateTime.UtcNow;

            _context.RentalAgreements.Add(agreement);
            await _context.SaveChangesAsync();

            return await GetAgreementByIdAsync(agreement.Id)
                ?? throw new Exception("Agreement creation failed");
        }

        public async Task<RentalAgreementDTO> UpdateAgreementAsync(int id, UpdateRentalAgreementDTO dto)
        {
            var agreement = await _context.RentalAgreements.FindAsync(id);

            if (agreement == null)
                throw new Exception($"Agreement with id {id} not found");

            _mapper.Map(dto, agreement);
            await _context.SaveChangesAsync();

            return await GetAgreementByIdAsync(agreement.Id)
                ?? throw new Exception("Agreement update failed");
        }

        public async Task<bool> CompleteAgreementAsync(int id, UpdateRentalAgreementDTO dto)
        {
            var agreement = await _context.RentalAgreements.FindAsync(id);

            if (agreement == null)
                return false;

            // İade bilgilerini güncelle
            agreement.ReturnKilometer = dto.ReturnKilometer;
            agreement.FuelLevelAtReturn = dto.FuelLevelAtReturn;
            agreement.ReturnNotes = dto.ReturnNotes;
            agreement.ReturnDateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAgreementAsync(int id)
        {
            var agreement = await _context.RentalAgreements.FindAsync(id);

            if (agreement == null)
                return false;

            _context.RentalAgreements.Remove(agreement);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}