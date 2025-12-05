using RentCarSystem.Application.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface ICategoryService

    {
        // Query (Okuma)
        Task<List<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO?> GetCategoryByIdAsync(int id);
        Task<List<CategoryDTO>> GetActiveCategoriesAsync();

        // Command (Yazma)
        Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO dto);
        Task<CategoryDTO> UpdateCategoryAsync(int id, CreateCategoryDTO dto);
        Task<bool> DeleteCategoryAsync(int id);


    }
}
