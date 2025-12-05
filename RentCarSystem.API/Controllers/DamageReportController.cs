using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DamageReportController : ControllerBase
    {
        private readonly IDamageReportService _damageReportService;

        public DamageReportController(IDamageReportService damageReportService)
        {
            _damageReportService = damageReportService;
        }

        /// <summary>
        /// Tüm hasar raporlarını listeler
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DamageReportDTO>>> GetAll()
        {
            var reports = await _damageReportService.GetAllReportsAsync();
            return Ok(reports);
        }

        /// <summary>
        /// ID'ye göre hasar raporu getirir
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DamageReportDTO>> GetById(int id)
        {
            var report = await _damageReportService.GetReportByIdAsync(id);

            if (report == null)
                return NotFound(new { message = $"DamageReport with id {id} not found" });

            return Ok(report);
        }

        /// <summary>
        /// Rezervasyona göre hasar raporlarını listeler
        /// </summary>
        [HttpGet("reservation/{reservationId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DamageReportDTO>>> GetByReservation(int reservationId)
        {
            var reports = await _damageReportService.GetReportsByReservationAsync(reservationId);
            return Ok(reports);
        }

        /// <summary>
        /// Sözleşmeye göre hasar raporlarını listeler
        /// </summary>
        [HttpGet("agreement/{agreementId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DamageReportDTO>>> GetByAgreement(int agreementId)
        {
            var reports = await _damageReportService.GetReportsByAgreementAsync(agreementId);
            return Ok(reports);
        }

        /// <summary>
        /// Araca göre hasar raporlarını listeler
        /// </summary>
        [HttpGet("vehicle/{vehicleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DamageReportDTO>>> GetByVehicle(int vehicleId)
        {
            var reports = await _damageReportService.GetReportsByVehicleAsync(vehicleId);
            return Ok(reports);
        }

        /// <summary>
        /// Duruma göre hasar raporlarını listeler
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<DamageReportDTO>>> GetByStatus(string status)
        {
            var reports = await _damageReportService.GetReportsByStatusAsync(status);
            return Ok(reports);
        }

        /// <summary>
        /// Yeni hasar raporu oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DamageReportDTO>> Create([FromBody] CreateDamageReportDTO dto)
        {
            var report = await _damageReportService.CreateReportAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
        }

        /// <summary>
        /// Hasar raporu günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DamageReportDTO>> Update(int id, [FromBody] UpdateDamageReportDTO dto)
        {
            var report = await _damageReportService.UpdateReportAsync(id, dto);
            return Ok(report);
        }

        /// <summary>
        /// Hasar raporunu çözümler
        /// </summary>
        [HttpPost("{id}/resolve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Resolve(int id, [FromBody] ResolveDamageDTO dto)
        {
            var result = await _damageReportService.ResolveReportAsync(id, dto.Resolution, dto.ActualCost);

            if (!result)
                return NotFound(new { message = $"DamageReport with id {id} not found" });

            return Ok(new { message = "DamageReport resolved successfully" });
        }

        /// <summary>
        /// Hasar raporu siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _damageReportService.DeleteReportAsync(id);

            if (!result)
                return NotFound(new { message = $"DamageReport with id {id} not found" });

            return NoContent();
        }
    }
}