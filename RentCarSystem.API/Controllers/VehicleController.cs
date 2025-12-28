using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        /// <summary>
        /// Araçları filtreleme ve sayfalandırma ile listeler
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<VehicleDTO>>> GetVehicles(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? query = null,
            [FromQuery] int? brandId = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? color = null,
            [FromQuery] string? status = null)
        {
            var filter = new FilterVehiclesDTO
            {
                PageNumber = page,
                PageSize = pageSize,
                SearchTerm = query,
                BrandId = brandId,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Color = color,
                Status = status
            };

            var result = await _vehicleService.GetFilteredVehiclesAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// ID'ye göre araç getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDTO>> GetById(int id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);

            if (vehicle == null)
                return NotFound(new { message = $"Vehicle with id {id} not found" });

            return Ok(vehicle);
        }

        /// <summary>
        /// Müsait araçları listeler
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<List<VehicleDTO>>> GetAvailable()
        {
            var vehicles = await _vehicleService.GetAvailableVehiclesAsync();
            return Ok(vehicles);
        }

        /// <summary>
        /// Markaya göre araçları listeler
        /// </summary>
        [HttpGet("brand/{brandId}")]
        public async Task<ActionResult<List<VehicleDTO>>> GetByBrand(int brandId)
        {
            var vehicles = await _vehicleService.GetVehiclesByBrandAsync(brandId);
            return Ok(vehicles);
        }

        /// <summary>
        /// Kategoriye göre araçları listeler
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<List<VehicleDTO>>> GetByCategory(int categoryId)
        {
            var vehicles = await _vehicleService.GetVehiclesByCategoryAsync(categoryId);
            return Ok(vehicles);
        }

        /// <summary>
        /// Yeni araç ekler
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VehicleDTO>> Create([FromBody] CreateVehicleDTO dto)
        {
            var vehicle = await _vehicleService.CreateVehicleAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
        }

        /// <summary>
        /// Araç günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VehicleDTO>> Update(int id, [FromBody] CreateVehicleDTO dto)
        {
            var vehicle = await _vehicleService.UpdateVehicleAsync(id, dto);
            return Ok(vehicle);
        }

        /// <summary>
        /// Araç siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);

            if (!result)
                return NotFound(new { message = $"Vehicle with id {id} not found" });

            return NoContent();
        }

        /// <summary>
        /// Elasticsearch ile araç ara
        /// </summary>
        [HttpGet("search-elasticsearch")]
        public async Task<IActionResult> SearchVehicles([FromQuery] string query)
        {
            var result = await _vehicleService.SearchVehiclesAsync(query);
            return Ok(result);
        }
    }
}