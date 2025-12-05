using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsuranceController : ControllerBase
    {
        private readonly IInsuranceService _insuranceService;

        public InsuranceController(IInsuranceService insuranceService)
        {
            _insuranceService = insuranceService;
        }

        /// <summary>
        /// Tüm sigorta poliçelerini listeler
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<InsuranceDTO>>> GetAll()
        {
            var insurances = await _insuranceService.GetAllInsurancesAsync();
            return Ok(insurances);
        }

        /// <summary>
        /// ID'ye göre sigorta poliçesi getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<InsuranceDTO>> GetById(int id)
        {
            var insurance = await _insuranceService.GetInsuranceByIdAsync(id);

            if (insurance == null)
                return NotFound(new { message = $"Insurance with id {id} not found" });

            return Ok(insurance);
        }

        /// <summary>
        /// Araca göre sigorta poliçelerini listeler
        /// </summary>
        [HttpGet("vehicle/{vehicleId}")]
        public async Task<ActionResult<List<InsuranceDTO>>> GetByVehicle(int vehicleId)
        {
            var insurances = await _insuranceService.GetInsurancesByVehicleAsync(vehicleId);
            return Ok(insurances);
        }

        /// <summary>
        /// Aktif sigorta poliçelerini listeler
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<List<InsuranceDTO>>> GetActive()
        {
            var insurances = await _insuranceService.GetActiveInsurancesAsync();
            return Ok(insurances);
        }

        /// <summary>
        /// Süresi yaklaşan sigorta poliçelerini listeler
        /// </summary>
        [HttpGet("expiring")]
        public async Task<ActionResult<List<InsuranceDTO>>> GetExpiring([FromQuery] int days = 30)
        {
            var insurances = await _insuranceService.GetExpiringInsurancesAsync(days);
            return Ok(insurances);
        }

        /// <summary>
        /// Yeni sigorta poliçesi oluşturur
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<InsuranceDTO>> Create([FromBody] CreateInsuranceDTO dto)
        {
            var insurance = await _insuranceService.CreateInsuranceAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = insurance.Id }, insurance);
        }

        /// <summary>
        /// Sigorta poliçesi günceller
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<InsuranceDTO>> Update(int id, [FromBody] CreateInsuranceDTO dto)
        {
            var insurance = await _insuranceService.UpdateInsuranceAsync(id, dto);
            return Ok(insurance);
        }

        /// <summary>
        /// Sigorta poliçesi siler
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _insuranceService.DeleteInsuranceAsync(id);

            if (!result)
                return NotFound(new { message = $"Insurance with id {id} not found" });

            return NoContent();
        }
    }
}