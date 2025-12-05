using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IDamageImageService
    {
        Task<List<DamageImageDTO>> GetAllImagesAsync();
        Task<DamageImageDTO?> GetImageByIdAsync(int id);
        Task<List<DamageImageDTO>> GetImagesByReportAsync(int reportId);

        Task<DamageImageDTO> CreateImageAsync(CreateDamageImageDTO dto);
        Task<DamageImageDTO> UpdateImageAsync(int id, CreateDamageImageDTO dto);
        Task<bool> DeleteImageAsync(int id);
    }
}