using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        /// <summary>
        /// Tüm markaları listeler
        /// </summary>
        [HttpGet]
        [AllowAnonymous]  
        public async Task<ActionResult<List<BrandDTO>>> GetAll()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(brands);
        }

        /// <summary>
        /// ID'ye göre marka getirir
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]  
        public async Task<ActionResult<BrandDTO>> GetById(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
                return NotFound(new { message = $"Brand with id {id} not found" });
            return Ok(brand);
        }

        /// <summary>
        /// Aktif markaları listeler
        /// </summary>
        [HttpGet("active")]
        [AllowAnonymous]  
        public async Task<ActionResult<List<BrandDTO>>> GetActive()
        {
            var brands = await _brandService.GetActiveBrandsAsync();
            return Ok(brands);
        }

        /// <summary>
        /// Yeni marka ekler
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]  
        public async Task<ActionResult<BrandDTO>> Create([FromBody] CreateBrandDTO dto)
        {
            var brand = await _brandService.CreateBrandAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
        }

        /// <summary>
        /// Marka günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]  
        public async Task<ActionResult<BrandDTO>> Update(int id, [FromBody] CreateBrandDTO dto)
        {
            var brand = await _brandService.UpdateBrandAsync(id, dto);
            return Ok(brand);
        }

        /// <summary>
        /// Marka siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _brandService.DeleteBrandAsync(id);
            if (!result)
                return NotFound(new { message = $"Brand with id {id} not found" });
            return NoContent();
        }
    }
}