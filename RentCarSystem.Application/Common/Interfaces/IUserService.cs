using RentCarSystem.Application.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<UserDTO?> GetUserByEmailAsync(string email);
        Task<List<UserDTO>> GetActiveUsersAsync();

        // Command (Yazma)
        Task<UserDTO> CreateUserAsync(UpdateUserDTO dto);      // CreateUserAsync (TEKİL)
        Task<UserDTO> UpdateUserAsync(int id, UpdateUserDTO dto);  // UpdateUserAsync (TEKİL)
        Task<bool> DeleteUserAsync(int id);
        Task<UserDTO> GetUserByPublicIdAsync(Guid publicId);
        Task<UserDTO> UpdateUserByPublicIdAsync(Guid publicId, UpdateUserDTO dto);
        Task<bool> DeleteUserByPublicIdAsync(Guid publicId);
        Task<bool> ToggleUserActiveStatusAsync(Guid publicId);


    }
}
