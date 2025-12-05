using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalAgreementController : ControllerBase
    {
        private readonly IRentalAgreementService _agreementService;

        public RentalAgreementController(IRentalAgreementService agreementService)
        {
            _agreementService = agreementService;
        }

        /// <summary>
        /// Tüm kiralama sözleşmelerini listeler
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<RentalAgreementDTO>>> GetAll()
        {
            var agreements = await _agreementService.GetAllAgreementsAsync();
            return Ok(agreements);
        }

        /// <summary>
        /// ID'ye göre sözleşme getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RentalAgreementDTO>> GetById(int id)
        {
            var agreement = await _agreementService.GetAgreementByIdAsync(id);

            if (agreement == null)
                return NotFound(new { message = $"RentalAgreement with id {id} not found" });

            return Ok(agreement);
        }

        /// <summary>
        /// Rezervasyona göre sözleşme getirir
        /// </summary>
        [HttpGet("reservation/{reservationId}")]
        public async Task<ActionResult<RentalAgreementDTO>> GetByReservation(int reservationId)
        {
            var agreement = await _agreementService.GetAgreementByReservationAsync(reservationId);

            if (agreement == null)
                return NotFound(new { message = $"RentalAgreement for reservation {reservationId} not found" });

            return Ok(agreement);
        }

        /// <summary>
        /// Aktif sözleşmeleri listeler (teslim edilmiş, henüz iade edilmemiş)
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<List<RentalAgreementDTO>>> GetActive()
        {
            var agreements = await _agreementService.GetActiveAgreementsAsync();
            return Ok(agreements);
        }

        /// <summary>
        /// Tamamlanmış sözleşmeleri listeler
        /// </summary>
        [HttpGet("completed")]
        public async Task<ActionResult<List<RentalAgreementDTO>>> GetCompleted()
        {
            var agreements = await _agreementService.GetCompletedAgreementsAsync();
            return Ok(agreements);
        }

        /// <summary>
        /// Yeni kiralama sözleşmesi oluşturur (Araç Teslimi)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<RentalAgreementDTO>> Create([FromBody] CreateRentalAgreementDTO dto)
        {
            var agreement = await _agreementService.CreateAgreementAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = agreement.Id }, agreement);
        }

        /// <summary>
        /// Sözleşme günceller
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<RentalAgreementDTO>> Update(int id, [FromBody] UpdateRentalAgreementDTO dto)
        {
            var agreement = await _agreementService.UpdateAgreementAsync(id, dto);
            return Ok(agreement);
        }

        /// <summary>
        /// Sözleşmeyi tamamlar (Araç İadesi)
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<ActionResult> Complete(int id, [FromBody] UpdateRentalAgreementDTO dto)
        {
            var result = await _agreementService.CompleteAgreementAsync(id, dto);

            if (!result)
                return NotFound(new { message = $"RentalAgreement with id {id} not found" });

            return Ok(new { message = "RentalAgreement completed successfully" });
        }

        /// <summary>
        /// Sözleşme siler
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _agreementService.DeleteAgreementAsync(id);

            if (!result)
                return NotFound(new { message = $"RentalAgreement with id {id} not found" });

            return NoContent();
        }
    }
}