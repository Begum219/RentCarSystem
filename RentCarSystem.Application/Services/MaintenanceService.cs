using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;


namespace RentCarSystem.Application.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MaintenanceService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<MaintenanceDTO>> GetAllMaintenancesAsync()
        {
            var maintenances = await _context.Maintenances
                .Include(m => m.Vehicle)
                .ToListAsync();

            return _mapper.Map<List<MaintenanceDTO>>(maintenances);
        }

        public async Task<MaintenanceDTO?> GetMaintenanceByIdAsync(int id)
        {
            var maintenance = await _context.Maintenances
                .Include(m => m.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);

            return _mapper.Map<MaintenanceDTO>(maintenance);
        }

        public async Task<List<MaintenanceDTO>> GetMaintenancesByVehicleAsync(int vehicleId)
        {
            var maintenances = await _context.Maintenances
                .Include(m => m.Vehicle)
                .Where(m => m.VehicleId == vehicleId)
                .ToListAsync();

            return _mapper.Map<List<MaintenanceDTO>>(maintenances);
        }

        public async Task<List<MaintenanceDTO>> GetActiveMaintenancesAsync()
        {
            var maintenances = await _context.Maintenances
                .Include(m => m.Vehicle)
                .Where(m => !m.IsCompleted)
                .ToListAsync();

            return _mapper.Map<List<MaintenanceDTO>>(maintenances);
        }

        public async Task<List<MaintenanceDTO>> GetCompletedMaintenancesAsync()
        {
            var maintenances = await _context.Maintenances
                .Include(m => m.Vehicle)
                .Where(m => m.IsCompleted)
                .ToListAsync();

            return _mapper.Map<List<MaintenanceDTO>>(maintenances);
        }

        public async Task<MaintenanceDTO> CreateMaintenanceAsync(CreateMaintenanceDTO dto)
        {
            var maintenance = _mapper.Map<Maintenance>(dto);

            _context.Maintenances.Add(maintenance);
            await _context.SaveChangesAsync();

            return await GetMaintenanceByIdAsync(maintenance.Id)
                ?? throw new Exception("Maintenance creation failed");
        }

        public async Task<MaintenanceDTO> UpdateMaintenanceAsync(int id, CreateMaintenanceDTO dto)
        {
            var maintenance = await _context.Maintenances.FindAsync(id);

            if (maintenance == null)
                throw new Exception($"Maintenance with id {id} not found");

            _mapper.Map(dto, maintenance);
            await _context.SaveChangesAsync();

            return await GetMaintenanceByIdAsync(maintenance.Id)
                ?? throw new Exception("Maintenance update failed");
        }

        public async Task<bool> CompleteMaintenanceAsync(int id)
        {
            var maintenance = await _context.Maintenances.FindAsync(id);

            if (maintenance == null)
                return false;

            maintenance.IsCompleted = true;
            maintenance.EndDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteMaintenanceAsync(int id)
        {
            var maintenance = await _context.Maintenances.FindAsync(id);

            if (maintenance == null)
                return false;

            _context.Maintenances.Remove(maintenance);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}