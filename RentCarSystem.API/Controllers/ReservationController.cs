using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using System.Security.Claims;
using RentCarSystem.Application.Orchestrators;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ReservationOrchestrator _orchestrator;
        private readonly ApplicationDbContext _context;

        public ReservationController(
            IReservationService reservationService,
            ReservationOrchestrator orchestrator,
            ApplicationDbContext context)
        {
            _reservationService = reservationService;
            _orchestrator = orchestrator;
            _context = context;
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
        /// PublicId'ye göre rezervasyon getirir
        /// </summary>
        [HttpGet("{publicId}")]
        [Authorize]
        public async Task<ActionResult<ReservationDTO>> GetById(Guid publicId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.PublicId == publicId);

            if (reservation == null)
                return NotFound(new { message = $"Reservation with id {publicId} not found" });

            // IDOR Protection: Kullanıcı sadece kendi rezervasyonunu görebilir
            if (!isAdmin && reservation.UserId != userId)
                return Forbid();

            var dto = await _reservationService.GetReservationByPublicIdAsync(publicId);
            return Ok(dto);
        }

        /// <summary>
        /// Kullanıcıya göre rezervasyonları listeler
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ReservationDTO>>> GetByUser(int userId)
        {
            var reservations = await _reservationService.GetReservationsByUserAsync(userId);
            return Ok(reservations);
        }

        /// <summary>
        /// Araca göre rezervasyonları listeler
        /// </summary>
        [HttpGet("vehicle/{vehicleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ReservationDTO>>> GetByVehicle(int vehicleId)
        {
            var reservations = await _reservationService.GetReservationsByVehicleAsync(vehicleId);
            return Ok(reservations);
        }

        /// <summary>
        /// Aktif rezervasyonları listeler
        /// </summary>
        [HttpGet("active")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ReservationDTO>>> GetActive()
        {
            var reservations = await _reservationService.GetActiveReservationsAsync();
            return Ok(reservations);
        }

        /// <summary>
        /// Bekleyen rezervasyonları listeler
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
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
            return CreatedAtAction(nameof(GetById), new { publicId = reservation.PublicId }, reservation);
        }

        /// <summary>
        /// Rezervasyon günceller
        /// </summary>
        [HttpPut("{publicId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ReservationDTO>> Update(Guid publicId, [FromBody] UpdateReservationDTO dto)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.PublicId == publicId);

            if (reservation == null)
                return NotFound(new { message = $"Reservation with id {publicId} not found" });

            var updated = await _reservationService.UpdateReservationAsync(reservation.Id, dto);
            return Ok(updated);
        }

        /// <summary>
        /// Rezervasyonu iptal eder
        /// </summary>
        [HttpPost("{publicId}/cancel")]
        [Authorize]
        public async Task<ActionResult> Cancel(Guid publicId, [FromBody] CancelReservationDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.PublicId == publicId);

            if (reservation == null)
                return NotFound(new { message = $"Reservation with id {publicId} not found" });

            // IDOR Protection: Kullanıcı sadece kendi rezervasyonunu iptal edebilir
            if (!isAdmin && reservation.UserId != userId)
                return Forbid();

            var result = await _reservationService.CancelReservationAsync(reservation.Id, dto.Reason);

            if (!result)
                return NotFound(new { message = $"Reservation with id {publicId} not found" });

            return Ok(new { message = "Reservation cancelled successfully" });
        }

        /// <summary>
        /// Rezervasyonu tamamlar
        /// </summary>
        [HttpPost("{publicId}/complete")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Complete(Guid publicId)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.PublicId == publicId);

            if (reservation == null)
                return NotFound(new { message = $"Reservation with id {publicId} not found" });

            var result = await _reservationService.CompleteReservationAsync(reservation.Id);

            if (!result)
                return NotFound(new { message = $"Reservation with id {publicId} not found" });

            return Ok(new { message = "Reservation completed successfully" });
        }

        /// <summary>
        /// Rezervasyon siler
        /// </summary>
        [HttpDelete("{publicId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid publicId)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.PublicId == publicId);

            if (reservation == null)
                return NotFound(new { message = $"Reservation with id {publicId} not found" });

            var result = await _reservationService.DeleteReservationAsync(reservation.Id);

            if (!result)
                return NotFound(new { message = $"Reservation with id {publicId} not found" });

            return NoContent();
        }

        /// <summary>
        /// Kendi rezervasyonları (Giriş yapmış kullanıcı)
        /// </summary>
        [HttpGet("my-reservations")]
        [Authorize]
        public async Task<ActionResult<List<ReservationDTO>>> GetMyReservations()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var reservations = await _reservationService.GetReservationsByUserAsync(userId);
            return Ok(reservations);
        }

        /// <summary>
        /// Rezervasyon + Ödeme oluştur
        /// </summary>
        [HttpPost("with-payment")]
        [Authorize]
        public async Task<ActionResult> CreateWithPayment([FromBody] CreateReservationWithPaymentDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _orchestrator.CreateReservationWithPaymentAsync(userId, dto);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}