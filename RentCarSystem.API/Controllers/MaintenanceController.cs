using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        /// <summary>
        /// Tüm bakımları listeler
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<MaintenanceDTO>>> GetAll()
        {
            var maintenances = await _maintenanceService.GetAllMaintenancesAsync();
            return Ok(maintenances);
        }

        /// <summary>
        /// ID'ye göre bakım getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenanceDTO>> GetById(int id)
        {
            var maintenance = await _maintenanceService.GetMaintenanceByIdAsync(id);

            if (maintenance == null)
                return NotFound(new { message = $"Maintenance with id {id} not found" });

            return Ok(maintenance);
        }

        /// <summary>
        /// Araca göre bakımları listeler
        /// </summary>
        [HttpGet("vehicle/{vehicleId}")]

        public async Task<ActionResult<List<MaintenanceDTO>>> GetByVehicle(int vehicleId)
        {
            var maintenances = await _maintenanceService.GetMaintenancesByVehicleAsync(vehicleId);
            return Ok(maintenances);
        }

        /// <summary>
        /// Aktif bakımları listeler (devam eden)
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<List<MaintenanceDTO>>> GetActive()
        {
            var maintenances = await _maintenanceService.GetActiveMaintenancesAsync();
            return Ok(maintenances);
        }

        /// <summary>
        /// Tamamlanmış bakımları listeler
        /// </summary>
        [HttpGet("completed")]
        public async Task<ActionResult<List<MaintenanceDTO>>> GetCompleted()
        {
            var maintenances = await _maintenanceService.GetCompletedMaintenancesAsync();
            return Ok(maintenances);
        }

        /// <summary>
        /// Yeni bakım kaydı oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MaintenanceDTO>> Create([FromBody] CreateMaintenanceDTO dto)
        {
            var maintenance = await _maintenanceService.CreateMaintenanceAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = maintenance.Id }, maintenance);
        }

        /// <summary>
        /// Bakım kaydı günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MaintenanceDTO>> Update(int id, [FromBody] CreateMaintenanceDTO dto)
        {
            var maintenance = await _maintenanceService.UpdateMaintenanceAsync(id, dto);
            return Ok(maintenance);
        }

        /// <summary>
        /// Bakımı tamamlar
        /// </summary>
        [HttpPost("{id}/complete")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Complete(int id)
        {
            var result = await _maintenanceService.CompleteMaintenanceAsync(id);

            if (!result)
                return NotFound(new { message = $"Maintenance with id {id} not found" });

            return Ok(new { message = "Maintenance completed successfully" });
        }

        /// <summary>
        /// Bakım kaydı siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _maintenanceService.DeleteMaintenanceAsync(id);

            if (!result)
                return NotFound(new { message = $"Maintenance with id {id} not found" });

            return NoContent();
        }
    }
}