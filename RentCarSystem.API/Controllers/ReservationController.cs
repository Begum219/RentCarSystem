using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using System.Security.Claims;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        /// <summary>
        /// Tüm rezervasyonları listeler
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ReservationDTO>>> GetAll()
        {
            var reservations = await _reservationService.GetAllReservationsAsync();
            return Ok(reservations);
        }

        /// <summary>
        /// ID'ye göre rezervasyon getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetById(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);

            if (reservation == null)
                return NotFound(new { message = $"Reservation with id {id} not found" });

            return Ok(reservation);
        }

        /// <summary>
        /// Kullanıcıya göre rezervasyonları listeler
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ReservationDTO>>> GetByUser(int userId)
        {
            var reservations = await _reservationService.GetReservationsByUserAsync(userId);
            return Ok(reservations);
        }

        /// <summary>
        /// Araca göre rezervasyonları listeler
        /// </summary>
        [HttpGet("vehicle/{vehicleId}")]
        public async Task<ActionResult<List<ReservationDTO>>> GetByVehicle(int vehicleId)
        {
            var reservations = await _reservationService.GetReservationsByVehicleAsync(vehicleId);
            return Ok(reservations);
        }

        /// <summary>
        /// Aktif rezervasyonları listeler
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<List<ReservationDTO>>> GetActive()
        {
            var reservations = await _reservationService.GetActiveReservationsAsync();
            return Ok(reservations);
        }

        /// <summary>
        /// Bekleyen rezervasyonları listeler
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<List<ReservationDTO>>> GetPending()
        {
            var reservations = await _reservationService.GetPendingReservationsAsync();
            return Ok(reservations);
        }

        /// <summary>
        /// Yeni rezervasyon oluşturur
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReservationDTO>> Create([FromBody] CreateReservationDTO dto)
        {
            var reservation = await _reservationService.CreateReservationAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
        }

        /// <summary>
        /// Rezervasyon günceller
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ReservationDTO>> Update(int id, [FromBody] UpdateReservationDTO dto)
        {
            var reservation = await _reservationService.UpdateReservationAsync(id, dto);
            return Ok(reservation);
        }

        /// <summary>
        /// Rezervasyonu iptal eder
        /// </summary>
        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<ActionResult> Cancel(int id, [FromBody] CancelReservationDTO dto)
        {
            var result = await _reservationService.CancelReservationAsync(id, dto.Reason);

            if (!result)
                return NotFound(new { message = $"Reservation with id {id} not found" });

            return Ok(new { message = "Reservation cancelled successfully" });
        }

        /// <summary>
        /// Rezervasyonu tamamlar
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<ActionResult> Complete(int id)
        {
            var result = await _reservationService.CompleteReservationAsync(id);

            if (!result)
                return NotFound(new { message = $"Reservation with id {id} not found" });

            return Ok(new { message = "Reservation completed successfully" });
        }

        /// <summary>
        /// Rezervasyon siler
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reservationService.DeleteReservationAsync(id);

            if (!result)
                return NotFound(new { message = $"Reservation with id {id} not found" });

            return NoContent();
        }
        // Kendi rezervasyonları (Giriş yapmış kullanıcı)
        [HttpGet("my-reservations")]
        [Authorize]
        public async Task<ActionResult<List<ReservationDTO>>> GetMyReservations()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var reservations = await _reservationService.GetReservationsByUserAsync(userId);
            return Ok(reservations);
        }

    }
}