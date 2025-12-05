using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.Application.Services
{
    public class InsuranceService : IInsuranceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public InsuranceService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<InsuranceDTO>> GetAllInsurancesAsync()
        {
            var insurances = await _context.Insurances
                .Include(i => i.Vehicle)
                .Where(i => i.IsActive)
                .ToListAsync();

            return _mapper.Map<List<InsuranceDTO>>(insurances);
        }

        public async Task<InsuranceDTO?> GetInsuranceByIdAsync(int id)
        {
            var insurance = await _context.Insurances
                .Include(i => i.Vehicle)
                .Where(i => i.Id == id && i.IsActive)
                .FirstOrDefaultAsync();

            return _mapper.Map<InsuranceDTO>(insurance);
        }

        public async Task<List<InsuranceDTO>> GetInsurancesByVehicleAsync(int vehicleId)
        {
            var insurances = await _context.Insurances
                .Include(i => i.Vehicle)
                .Where(i => i.VehicleId == vehicleId && i.IsActive)
                .ToListAsync();

            return _mapper.Map<List<InsuranceDTO>>(insurances);
        }

        public async Task<List<InsuranceDTO>> GetActiveInsurancesAsync()
        {
            var now = DateTime.UtcNow;
            var insurances = await _context.Insurances
                .Include(i => i.Vehicle)
                .Where(i => i.IsActive && i.StartDate <= now && i.EndDate >= now)
                .ToListAsync();

            return _mapper.Map<List<InsuranceDTO>>(insurances);
        }

        public async Task<List<InsuranceDTO>> GetExpiringInsurancesAsync(int days)
        {
            var futureDate = DateTime.UtcNow.AddDays(days);
            var insurances = await _context.Insurances
                .Include(i => i.Vehicle)
                .Where(i => i.IsActive && i.EndDate <= futureDate && i.EndDate >= DateTime.UtcNow)
                .ToListAsync();

            return _mapper.Map<List<InsuranceDTO>>(insurances);
        }

        public async Task<InsuranceDTO> CreateInsuranceAsync(CreateInsuranceDTO dto)
        {
            var insurance = _mapper.Map<Insurance>(dto);
            insurance.IsActive = true;

            _context.Insurances.Add(insurance);
            await _context.SaveChangesAsync();

            return await GetInsuranceByIdAsync(insurance.Id)
                ?? throw new Exception("Insurance creation failed");
        }

        public async Task<InsuranceDTO> UpdateInsuranceAsync(int id, CreateInsuranceDTO dto)
        {
            var insurance = await _context.Insurances.FindAsync(id);

            if (insurance == null)
                throw new Exception($"Insurance with id {id} not found");

            _mapper.Map(dto, insurance);
            await _context.SaveChangesAsync();

            return await GetInsuranceByIdAsync(insurance.Id)
                ?? throw new Exception("Insurance update failed");
        }

        public async Task<bool> DeleteInsuranceAsync(int id)
        {
            var insurance = await _context.Insurances.FindAsync(id);

            if (insurance == null)
                return false;

            insurance.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}