using AutoMapper;  
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper; 

        public BrandService(ApplicationDbContext context, IMapper mapper)  
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<BrandDTO>> GetAllBrandsAsync()
        {
            var brands = await _context.Brands
                .Where(b => b.IsActive)
                .ToListAsync();

            return _mapper.Map<List<BrandDTO>>(brands);  
        }

        public async Task<BrandDTO?> GetBrandByIdAsync(int id)
        {
            var brand = await _context.Brands
                .Where(b => b.Id == id && b.IsActive)
                .FirstOrDefaultAsync();

            return _mapper.Map<BrandDTO>(brand);  
        }

        public async Task<List<BrandDTO>> GetActiveBrandsAsync()
        {
            var brands = await _context.Brands
                .Where(b => b.IsActive)
                .ToListAsync();

            return _mapper.Map<List<BrandDTO>>(brands);  
        }

        public async Task<BrandDTO> CreateBrandAsync(CreateBrandDTO dto)
        {
            var brand = _mapper.Map<Brand>(dto);  
            brand.IsActive = true;

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return await GetBrandByIdAsync(brand.Id)
                ?? throw new Exception("Brand creation failed");
        }

        public async Task<BrandDTO> UpdateBrandAsync(int id, CreateBrandDTO dto)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
                throw new Exception($"Brand with id {id} not found");

            _mapper.Map(dto, brand);  

            await _context.SaveChangesAsync();

            return await GetBrandByIdAsync(brand.Id)
                ?? throw new Exception("Brand update failed");
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
                return false;

            brand.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}