using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Tüm lokasyonları listeler
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<LocationDTO>>> GetAll()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(locations);
        }

        /// <summary>
        /// ID'ye göre lokasyon getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationDTO>> GetById(int id)
        {
            var location = await _locationService.GetLocationByIdAsync(id);

            if (location == null)
                return NotFound(new { message = $"Location with id {id} not found" });

            return Ok(location);
        }

        /// <summary>
        /// Aktif lokasyonları listeler
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<List<LocationDTO>>> GetActive()
        {
            var locations = await _locationService.GetActiveLocationsAsync();
            return Ok(locations);
        }

        /// <summary>
        /// Yeni lokasyon ekler
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LocationDTO>> Create([FromBody] CreateLocationDTO dto)
        {
            var location = await _locationService.CreateLocationAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = location.Id }, location);
        }

        /// <summary>
        /// Lokasyon günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LocationDTO>> Update(int id, [FromBody] CreateLocationDTO dto)
        {
            var location = await _locationService.UpdateLocationAsync(id, dto);
            return Ok(location);
        }

        /// <summary>
        /// Lokasyon siler (Soft Delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _locationService.DeleteLocationAsync(id);

            if (!result)
                return NotFound(new { message = $"Location with id {id} not found" });

            return NoContent();
        }
    }
}