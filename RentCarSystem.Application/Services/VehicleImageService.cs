using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.Application.Services
{
    public class VehicleImageService : IVehicleImageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public VehicleImageService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<VehicleImageDTO>> GetAllImagesAsync()
        {
            var images = await _context.VehicleImages
                .Include(vi => vi.Vehicle)
                .OrderBy(vi => vi.DisplayOrder)
                .ToListAsync();

            return _mapper.Map<List<VehicleImageDTO>>(images);
        }

        public async Task<VehicleImageDTO?> GetImageByIdAsync(int id)
        {
            var image = await _context.VehicleImages
                .Include(vi => vi.Vehicle)
                .FirstOrDefaultAsync(vi => vi.Id == id);

            return _mapper.Map<VehicleImageDTO>(image);
        }

        public async Task<List<VehicleImageDTO>> GetImagesByVehicleAsync(int vehicleId)
        {
            var images = await _context.VehicleImages
                .Include(vi => vi.Vehicle)
                .Where(vi => vi.VehicleId == vehicleId)
                .OrderBy(vi => vi.DisplayOrder)
                .ToListAsync();

            return _mapper.Map<List<VehicleImageDTO>>(images);
        }

        public async Task<VehicleImageDTO?> GetMainImageAsync(int vehicleId)
        {
            var image = await _context.VehicleImages
                .Include(vi => vi.Vehicle)
                .Where(vi => vi.VehicleId == vehicleId && vi.IsMainImage)
                .FirstOrDefaultAsync();

            return _mapper.Map<VehicleImageDTO>(image);
        }

        public async Task<VehicleImageDTO> CreateImageAsync(CreateVehicleImageDTO dto)
        {
            // Eğer ana resim olarak işaretlendiyse, diğerlerini kaldır
            if (dto.IsMainImage)
            {
                var existingMainImages = await _context.VehicleImages
                    .Where(vi => vi.VehicleId == dto.VehicleId && vi.IsMainImage)
                    .ToListAsync();

                foreach (var img in existingMainImages)
                {
                    img.IsMainImage = false;
                }
            }

            var image = _mapper.Map<VehicleImage>(dto);

            _context.VehicleImages.Add(image);
            await _context.SaveChangesAsync();

            return await GetImageByIdAsync(image.Id)
                ?? throw new Exception("Image creation failed");
        }

        public async Task<VehicleImageDTO> UpdateImageAsync(int id, CreateVehicleImageDTO dto)
        {
            var image = await _context.VehicleImages.FindAsync(id);

            if (image == null)
                throw new Exception($"Image with id {id} not found");

            // Eğer ana resim olarak işaretlendiyse, diğerlerini kaldır
            if (dto.IsMainImage && !image.IsMainImage)
            {
                var existingMainImages = await _context.VehicleImages
                    .Where(vi => vi.VehicleId == dto.VehicleId && vi.IsMainImage)
                    .ToListAsync();

                foreach (var img in existingMainImages)
                {
                    img.IsMainImage = false;
                }
            }

            _mapper.Map(dto, image);
            await _context.SaveChangesAsync();

            return await GetImageByIdAsync(image.Id)
                ?? throw new Exception("Image update failed");
        }

        public async Task<bool> SetMainImageAsync(int imageId)
        {
            var image = await _context.VehicleImages.FindAsync(imageId);

            if (image == null)
                return false;

            // Aynı araçtaki diğer ana resimleri kaldır
            var existingMainImages = await _context.VehicleImages
                .Where(vi => vi.VehicleId == image.VehicleId && vi.IsMainImage)
                .ToListAsync();

            foreach (var img in existingMainImages)
            {
                img.IsMainImage = false;
            }

            // Bu resmi ana resim yap
            image.IsMainImage = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteImageAsync(int id)
        {
            var image = await _context.VehicleImages.FindAsync(id);

            if (image == null)
                return false;

            _context.VehicleImages.Remove(image);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}