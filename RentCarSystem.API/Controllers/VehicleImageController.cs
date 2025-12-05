using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleImageController : ControllerBase
    {
        private readonly IVehicleImageService _vehicleImageService;

        public VehicleImageController(IVehicleImageService vehicleImageService)
        {
            _vehicleImageService = vehicleImageService;
        }

        /// <summary>
        /// Tüm araç resimlerini listeler
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<VehicleImageDTO>>> GetAll()
        {
            var images = await _vehicleImageService.GetAllImagesAsync();
            return Ok(images);
        }

        /// <summary>
        /// ID'ye göre araç resmi getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleImageDTO>> GetById(int id)
        {
            var image = await _vehicleImageService.GetImageByIdAsync(id);

            if (image == null)
                return NotFound(new { message = $"VehicleImage with id {id} not found" });

            return Ok(image);
        }

        /// <summary>
        /// Araca göre resimleri listeler
        /// </summary>
        [HttpGet("vehicle/{vehicleId}")]
        public async Task<ActionResult<List<VehicleImageDTO>>> GetByVehicle(int vehicleId)
        {
            var images = await _vehicleImageService.GetImagesByVehicleAsync(vehicleId);
            return Ok(images);
        }

        /// <summary>
        /// Aracın ana resmini getirir
        /// </summary>
        [HttpGet("vehicle/{vehicleId}/main")]
        public async Task<ActionResult<VehicleImageDTO>> GetMainImage(int vehicleId)
        {
            var image = await _vehicleImageService.GetMainImageAsync(vehicleId);

            if (image == null)
                return NotFound(new { message = $"Main image for vehicle {vehicleId} not found" });

            return Ok(image);
        }

        /// <summary>
        /// Yeni araç resmi ekler
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VehicleImageDTO>> Create([FromBody] CreateVehicleImageDTO dto)
        {
            var image = await _vehicleImageService.CreateImageAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = image.Id }, image);
        }

        /// <summary>
        /// Araç resmi günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VehicleImageDTO>> Update(int id, [FromBody] CreateVehicleImageDTO dto)
        {
            var image = await _vehicleImageService.UpdateImageAsync(id, dto);
            return Ok(image);
        }

        /// <summary>
        /// Resmi ana resim yapar
        /// </summary>
        [HttpPost("{id}/set-main")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SetMain(int id)
        {
            var result = await _vehicleImageService.SetMainImageAsync(id);

            if (!result)
                return NotFound(new { message = $"VehicleImage with id {id} not found" });

            return Ok(new { message = "Image set as main successfully" });
        }

        /// <summary>
        /// Araç resmi siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _vehicleImageService.DeleteImageAsync(id);

            if (!result)
                return NotFound(new { message = $"VehicleImage with id {id} not found" });

            return NoContent();
        }
    }
}