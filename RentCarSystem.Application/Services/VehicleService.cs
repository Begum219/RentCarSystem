using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Domain.Enums;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public VehicleService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<VehicleDTO>> GetAllVehiclesAsync()
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Brand)     
                .Include(v => v.Category)  
                .Where(v => v.IsActive)
                .ToListAsync();

            return _mapper.Map<List<VehicleDTO>>(vehicles);  
        }

        public async Task<VehicleDTO?> GetVehicleByIdAsync(int id)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Category)
                .Where(v => v.Id == id && v.IsActive)
                .FirstOrDefaultAsync();

            return _mapper.Map<VehicleDTO>(vehicle);
        }

        public async Task<List<VehicleDTO>> GetAvailableVehiclesAsync()
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Category)
                .Where(v => v.IsActive && v.Status == VehicleStatus.Available)
                .ToListAsync();

            return _mapper.Map<List<VehicleDTO>>(vehicles);  
        }

        public async Task<List<VehicleDTO>> GetVehiclesByBrandAsync(int brandId)
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Category)
                .Where(v => v.BrandId == brandId && v.IsActive)
                .ToListAsync();

            return _mapper.Map<List<VehicleDTO>>(vehicles);
        }

        public async Task<List<VehicleDTO>> GetVehiclesByCategoryAsync(int categoryId)
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Category)
                .Where(v => v.CategoryId == categoryId && v.IsActive)
                .ToListAsync();

            return _mapper.Map<List<VehicleDTO>>(vehicles);  
        }

        public async Task<VehicleDTO> CreateVehicleAsync(CreateVehicleDTO dto)
        {
            var vehicle = _mapper.Map<Vehicle>(dto);
            vehicle.Status = VehicleStatus.Available;
            vehicle.IsActive = true;

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return await GetVehicleByIdAsync(vehicle.Id)
                ?? throw new Exception("Vehicle creation failed");
        }

        public async Task<VehicleDTO> UpdateVehicleAsync(int id, CreateVehicleDTO dto)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
                throw new Exception($"Vehicle with id {id} not found");

            _mapper.Map(dto, vehicle);  

            await _context.SaveChangesAsync();

            return await GetVehicleByIdAsync(vehicle.Id)
                ?? throw new Exception("Vehicle update failed");
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
                return false;

            vehicle.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}