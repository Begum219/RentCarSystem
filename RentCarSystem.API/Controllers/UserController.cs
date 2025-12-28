using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using System.Security.Claims;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Tüm kullanıcıları listeler (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDTO>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new { success = true, data = users });
        }

        /// <summary>
        /// PublicId'ye göre kullanıcı getirir
        /// </summary>
        [HttpGet("{publicId}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetById(Guid publicId)
        {
            var user = await _userService.GetUserByPublicIdAsync(publicId);

            if (user == null)
                return NotFound(new { success = false, message = $"Kullanıcı bulunamadı: {publicId}" });

            // Kullanıcı sadece kendi profilini veya admin tüm profilleri görebilir
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && user.PublicId.ToString() != currentUserId)
                return Forbid();

            return Ok(new { success = true, data = user });
        }

        /// <summary>
        /// Email'e göre kullanıcı getirir (Admin only)
        /// </summary>
        [HttpGet("email/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> GetByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
                return NotFound(new { success = false, message = $"Kullanıcı bulunamadı: {email}" });

            return Ok(new { success = true, data = user });
        }

        /// <summary>
        /// Aktif kullanıcıları listeler (Admin only)
        /// </summary>
        [HttpGet("active")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDTO>>> GetActive()
        {
            var users = await _userService.GetActiveUsersAsync();
            return Ok(new { success = true, data = users });
        }

        /// <summary>
        /// Kendi profil bilgilerini getirir
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetCurrentUser()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound(new { success = false, message = "Kullanıcı bulunamadı" });

            return Ok(new { success = true, data = user });
        }

        /// <summary>
        /// Yeni kullanıcı ekler (Public - Register için)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Create([FromBody] UpdateUserDTO dto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(dto);
                return CreatedAtAction(nameof(GetById), new { publicId = user.PublicId }, new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Kullanıcı profil günceller
        /// </summary>
        [HttpPut("{publicId}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> Update(Guid publicId, [FromBody] UpdateUserDTO dto)
        {
            try
            {
                // Authorization: Sadece kendi profilini veya admin güncelleyebilir
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = User.IsInRole("Admin");

                var existingUser = await _userService.GetUserByPublicIdAsync(publicId);
                if (existingUser == null)
                    return NotFound(new { success = false, message = "Kullanıcı bulunamadı" });

                if (!isAdmin && existingUser.PublicId.ToString() != currentUserId)
                    return Forbid();

                // Internal ID ile güncelle (Service layer int Id kullanıyor olabilir)
                // Burada GetUserByPublicIdAsync ile internal Id'yi al
                var user = await _userService.UpdateUserByPublicIdAsync(publicId, dto);

                return Ok(new { success = true, data = user, message = "Profil güncellendi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Kullanıcı siler - Soft Delete (Admin only)
        /// </summary>
        [HttpDelete("{publicId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid publicId)
        {
            try
            {
                var result = await _userService.DeleteUserByPublicIdAsync(publicId);

                if (!result)
                    return NotFound(new { success = false, message = "Kullanıcı bulunamadı" });

                return Ok(new { success = true, message = "Kullanıcı silindi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Kullanıcıyı aktif/pasif yapar (Admin only)
        /// </summary>
        [HttpPatch("{publicId}/toggle-active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActive(Guid publicId)
        {
            try
            {
                var result = await _userService.ToggleUserActiveStatusAsync(publicId);

                if (!result)
                    return NotFound(new { success = false, message = "Kullanıcı bulunamadı" });

                return Ok(new { success = true, message = "Kullanıcı durumu güncellendi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}