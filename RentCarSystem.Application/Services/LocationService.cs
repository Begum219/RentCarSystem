using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        

        public LocationService(ApplicationDbContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<LocationDTO>> GetAllLocationsAsync()
        {
            var locations = await _context.Locations

               .Where(l => l.IsActive)
               
                .ToListAsync();
            return _mapper.Map<List<LocationDTO>>(locations);
        }

        public async Task<LocationDTO?> GetLocationByIdAsync(int id)
        {
            var location = await _context.Locations
               .Where(l => l.Id == id && l.IsActive)
                .FirstOrDefaultAsync();
            return _mapper.Map<LocationDTO>(location);
        }

        public async Task<List<LocationDTO>> GetActiveLocationsAsync()
        {
            var locations = await _context.Locations
                .Where(l => l.IsActive)
                
                .ToListAsync();
            return _mapper.Map<List<LocationDTO>>(locations);
        }

        public async Task<LocationDTO> CreateLocationAsync(CreateLocationDTO dto)
        {
            var location= _mapper.Map<Location>(dto);
            location.IsActive = true;

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return await GetLocationByIdAsync(location.Id)
                ?? throw new Exception("Location creation failed");
        }

        public async Task<LocationDTO> UpdateLocationAsync(int id, CreateLocationDTO dto)
        {
            var location = await _context.Locations.FindAsync(id);

            if (location == null)
                throw new Exception($"Location with id {id} not found");

            _mapper.Map(dto, location);

            await _context.SaveChangesAsync();

            return await GetLocationByIdAsync(location.Id)
                ?? throw new Exception("Location update failed");
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            var location = await _context.Locations.FindAsync(id);

            if (location == null)
                return false;

            // Soft Delete
            location.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}