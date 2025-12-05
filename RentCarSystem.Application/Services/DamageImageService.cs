using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.Application.Services
{
    public class DamageImageService : IDamageImageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DamageImageService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<DamageImageDTO>> GetAllImagesAsync()
        {
            var images = await _context.DamageImages
                .Include(di => di.DamageReport)
                .OrderBy(di => di.DisplayOrder)
                .ToListAsync();

            return _mapper.Map<List<DamageImageDTO>>(images);
        }

        public async Task<DamageImageDTO?> GetImageByIdAsync(int id)
        {
            var image = await _context.DamageImages
                .Include(di => di.DamageReport)
                .FirstOrDefaultAsync(di => di.Id == id);

            return _mapper.Map<DamageImageDTO>(image);
        }

        public async Task<List<DamageImageDTO>> GetImagesByReportAsync(int reportId)
        {
            var images = await _context.DamageImages
                .Include(di => di.DamageReport)
                .Where(di => di.DamageReportId == reportId)
                .OrderBy(di => di.DisplayOrder)
                .ToListAsync();

            return _mapper.Map<List<DamageImageDTO>>(images);
        }

        public async Task<DamageImageDTO> CreateImageAsync(CreateDamageImageDTO dto)
        {
            // DamageReport kontrolü
            var report = await _context.DamageReports.FindAsync(dto.DamageReportId);
            if (report == null)
                throw new Exception("DamageReport not found");

            var image = _mapper.Map<DamageImage>(dto);

            _context.DamageImages.Add(image);
            await _context.SaveChangesAsync();

            return await GetImageByIdAsync(image.Id)
                ?? throw new Exception("Image creation failed");
        }

        public async Task<DamageImageDTO> UpdateImageAsync(int id, CreateDamageImageDTO dto)
        {
            var image = await _context.DamageImages.FindAsync(id);

            if (image == null)
                throw new Exception($"Image with id {id} not found");

            _mapper.Map(dto, image);
            await _context.SaveChangesAsync();

            return await GetImageByIdAsync(image.Id)
                ?? throw new Exception("Image update failed");
        }

        public async Task<bool> DeleteImageAsync(int id)
        {
            var image = await _context.DamageImages.FindAsync(id);

            if (image == null)
                return false;

            _context.DamageImages.Remove(image);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}