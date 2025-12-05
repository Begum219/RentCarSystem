using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

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
        /// Tüm kullanıcıları listeler
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDTO>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// ID'ye göre kullanıcı getirir
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound(new { message = $"User with id {id} not found" }); 

            return Ok(user);
        }

        /// <summary>
        /// Email'e göre kullanıcı getirir
        /// </summary>
        [HttpGet("email/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> GetByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
                return NotFound(new { message = $"User with email {email} not found" });

            return Ok(user);
        }

        /// <summary>
        /// Aktif kullanıcıları listeler
        /// </summary>
        [HttpGet("active")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDTO>>> GetActive()
        {
            var users = await _userService.GetActiveUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Yeni kullanıcı ekler
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Create([FromBody] UpdateUserDTO dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        /// <summary>
        /// Kullanıcı günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> Update(int id, [FromBody] UpdateUserDTO dto)
        {
            var user = await _userService.UpdateUserAsync(id, dto);
            return Ok(user);
        }

        /// <summary>
        /// Kullanıcı siler (Soft Delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
                return NotFound(new { message = $"User with id {id} not found" });

            return NoContent();
        }
    }
}