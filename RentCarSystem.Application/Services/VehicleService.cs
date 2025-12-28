using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Domain.Enums;
using RentCarSystem.Infrastructure.Context;
using System.Text.Json;

namespace RentCarSystem.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IElasticsearchService _elasticsearchService;

        public VehicleService(ApplicationDbContext context, IMapper mapper, IElasticsearchService elasticsearchService)
        {
            _context = context;
            _mapper = mapper;
            _elasticsearchService = elasticsearchService;
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

            // Elasticsearch'e ekle
            try
            {
                var vehicleData = new
                {
                    vehicle.Id,
                    vehicle.Model,
                    Brand = vehicle.Brand?.Name,
                    Category = vehicle.Category?.Name,
                    vehicle.PlateNumber,
                    vehicle.Color,
                    vehicle.DailyPrice,
                    vehicle.Status
                };
                await _elasticsearchService.IndexVehicleAsync(vehicle.Id, JsonSerializer.Serialize(vehicleData));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Elasticsearch error: {ex.Message}");
            }

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

            // Elasticsearch'de güncelle
            try
            {
                var vehicleData = new
                {
                    vehicle.Id,
                    vehicle.Model,
                    vehicle.PlateNumber,
                    vehicle.Color,
                    vehicle.DailyPrice,
                    vehicle.Status
                };
                await _elasticsearchService.IndexVehicleAsync(vehicle.Id, JsonSerializer.Serialize(vehicleData));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Elasticsearch error: {ex.Message}");
            }

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

        // Elasticsearch ile arama
        public async Task<dynamic> SearchVehiclesAsync(string query)
        {
            return await _elasticsearchService.SearchVehiclesAsync(query);
        }

        // ✅ Pagination ile araçları getir
        public async Task<PaginatedResult<VehicleDTO>> GetVehiclesPaginatedAsync(int pageNumber, int pageSize)
        {
            // Validation
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Max 100

            var query = _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Category)
                .Where(v => v.IsActive);

            var totalCount = await query.CountAsync();

            var vehicles = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<VehicleDTO>>(vehicles);

            return new PaginatedResult<VehicleDTO>(dtos, pageNumber, pageSize, totalCount);
        }

        // ✅ Filtreleme + Pagination ile araçları getir
        public async Task<PaginatedResult<VehicleDTO>> GetFilteredVehiclesAsync(FilterVehiclesDTO filter)
        {
            var query = _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Category)
                .Where(v => v.IsActive);

            // 🔍 Arama Terimi (Model veya Plaka)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(v => v.Model.ToLower().Contains(searchTerm) ||
                                        v.PlateNumber.ToLower().Contains(searchTerm));
            }

            // 🏷️ Marka Filtresi
            if (filter.BrandId.HasValue)
                query = query.Where(v => v.BrandId == filter.BrandId);

            // 📂 Kategori Filtresi
            if (filter.CategoryId.HasValue)
                query = query.Where(v => v.CategoryId == filter.CategoryId);

            // 💰 Minimum Fiyat Filtresi
            if (filter.MinPrice.HasValue)
                query = query.Where(v => v.DailyPrice >= filter.MinPrice);

            // 💰 Maksimum Fiyat Filtresi
            if (filter.MaxPrice.HasValue)
                query = query.Where(v => v.DailyPrice <= filter.MaxPrice);

            // 🎨 Renk Filtresi
            if (!string.IsNullOrWhiteSpace(filter.Color))
                query = query.Where(v => v.Color.ToLower() == filter.Color.ToLower());

            // 📊 Durum Filtresi
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<VehicleStatus>(filter.Status, true, out var status))
                    query = query.Where(v => v.Status == status);
            }

            // 📍 Sayfalandırma Doğrulaması
            if (filter.PageNumber < 1) filter.PageNumber = 1;
            if (filter.PageSize < 1) filter.PageSize = 10;
            if (filter.PageSize > 100) filter.PageSize = 100; // Max 100

            // Toplam kayıt sayısı
            var totalCount = await query.CountAsync();

            // Sayfalandırılmış sonuçlar
            var vehicles = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<VehicleDTO>>(vehicles);

            return new PaginatedResult<VehicleDTO>(dtos, filter.PageNumber, filter.PageSize, totalCount);
        }
    }
}