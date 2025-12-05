using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;
using AutoMapper;

namespace RentCarSystem.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoryService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
               .Where(c => c.IsActive)
                
                .ToListAsync();
            return _mapper.Map<List<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Where(c => c.Id == id && c.IsActive)
                
                .FirstOrDefaultAsync();
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<List<CategoryDTO>> GetActiveCategoriesAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
               
                .ToListAsync();
            return _mapper.Map<List<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO dto)
        {
            var category = _mapper.Map<Category>(dto);
            category.IsActive = true;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return await GetCategoryByIdAsync(category.Id)
                ?? throw new Exception("Category creation failed");
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(int id, CreateCategoryDTO dto)   
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                throw new Exception($"Category with id {id} not found");

            _mapper.Map(dto, category);

            await _context.SaveChangesAsync();

            return await GetCategoryByIdAsync(category.Id)
                ?? throw new Exception("Category update failed");
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return false;

            category.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}