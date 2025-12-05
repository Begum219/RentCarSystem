using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IBrandService
    {
        //querry -okuma 
        Task<List<BrandDTO>>GetAllBrandsAsync();
        Task<BrandDTO?> GetBrandByIdAsync(int id);
        Task<List<BrandDTO>> GetActiveBrandsAsync();
        //command -yazma 
        Task<BrandDTO> CreateBrandAsync(CreateBrandDTO dto);  //Create
        Task<BrandDTO> UpdateBrandAsync(int id, CreateBrandDTO dto);  //update
        Task<bool> DeleteBrandAsync(int id); // delete kısmı

    }
}
