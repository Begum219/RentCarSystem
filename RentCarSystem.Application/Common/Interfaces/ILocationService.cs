using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface ILocationService
    {
        Task<List<LocationDTO>> GetAllLocationsAsync();
        Task<LocationDTO?> GetLocationByIdAsync(int id);
        Task<List<LocationDTO>> GetActiveLocationsAsync();

        Task<LocationDTO> CreateLocationAsync(CreateLocationDTO dto);
        Task<LocationDTO> UpdateLocationAsync(int id, CreateLocationDTO dto);
        Task<bool> DeleteLocationAsync(int id);

    }
}
