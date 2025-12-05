using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DamageImageController : ControllerBase
    {
        private readonly IDamageImageService _damageImageService;

        public DamageImageController(IDamageImageService damageImageService)
        {
            _damageImageService = damageImageService;
        }

        /// <summary>
        /// Tüm hasar resimlerini listeler
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DamageImageDTO>>> GetAll()
        {
            var images = await _damageImageService.GetAllImagesAsync();
            return Ok(images);
        }

        /// <summary>
        /// ID'ye göre hasar resmi getirir
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DamageImageDTO>> GetById(int id)
        {
            var image = await _damageImageService.GetImageByIdAsync(id);

            if (image == null)
                return NotFound(new { message = $"DamageImage with id {id} not found" });

            return Ok(image);
        }

        /// <summary>
        /// Hasar raporuna göre resimleri listeler
        /// </summary>
        [HttpGet("report/{reportId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DamageImageDTO>>> GetByReport(int reportId)
        {
            var images = await _damageImageService.GetImagesByReportAsync(reportId);
            return Ok(images);
        }

        /// <summary>
        /// Yeni hasar resmi ekler
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DamageImageDTO>> Create([FromBody] CreateDamageImageDTO dto)
        {
            var image = await _damageImageService.CreateImageAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = image.Id }, image);
        }

        /// <summary>
        /// Hasar resmi günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DamageImageDTO>> Update(int id, [FromBody] CreateDamageImageDTO dto)
        {
            var image = await _damageImageService.UpdateImageAsync(id, dto);
            return Ok(image);
        }

        /// <summary>
        /// Hasar resmi siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _damageImageService.DeleteImageAsync(id);

            if (!result)
                return NotFound(new { message = $"DamageImage with id {id} not found" });

            return NoContent();
        }
    }
}