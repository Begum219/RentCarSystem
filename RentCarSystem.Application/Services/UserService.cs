using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();

            return _mapper.Map<List<UserDTO>>(users);  // ✅ AutoMapper
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id && u.IsActive)
                .FirstOrDefaultAsync();

            return _mapper.Map<UserDTO>(user);  // ✅ AutoMapper
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .Where(u => u.Email == email && u.IsActive)
                .FirstOrDefaultAsync();

            return _mapper.Map<UserDTO>(user);  // ✅ AutoMapper
        }

        public async Task<List<UserDTO>> GetActiveUsersAsync()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();

            return _mapper.Map<List<UserDTO>>(users);  // ✅ AutoMapper
        }

        public async Task<UserDTO> CreateUserAsync(UpdateUserDTO dto)
        {
            var user = _mapper.Map<User>(dto);  // ✅ AutoMapper
            user.Role = "Customer";
            user.IsActive = true;
            user.PasswordHash = "TempPassword123";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return await GetUserByIdAsync(user.Id)
                ?? throw new Exception("User creation failed");
        }

        public async Task<UserDTO> UpdateUserAsync(int id, UpdateUserDTO dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                throw new Exception($"User with id {id} not found");

            _mapper.Map(dto, user);  // ✅ AutoMapper (DTO → Existing Entity)

            await _context.SaveChangesAsync();

            return await GetUserByIdAsync(user.Id)
                ?? throw new Exception("User update failed");
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return false;

            user.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}