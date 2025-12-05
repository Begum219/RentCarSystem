using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Tüm ödemeleri listeler
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<PaymentDTO>>> GetAll()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }

        /// <summary>
        /// ID'ye göre ödeme getirir
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PaymentDTO>> GetById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);

            if (payment == null)
                return NotFound(new { message = $"Payment with id {id} not found" });

            return Ok(payment);
        }

        /// <summary>
        /// Rezervasyona göre ödemeleri listeler
        /// </summary>
        [HttpGet("reservation/{reservationId}")]
        [Authorize]
        public async Task<ActionResult<List<PaymentDTO>>> GetByReservation(int reservationId)
        {
            var payments = await _paymentService.GetPaymentsByReservationAsync(reservationId);
            return Ok(payments);
        }

        /// <summary>
        /// Kullanıcıya göre ödemeleri listeler
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<PaymentDTO>>> GetByUser(int userId)
        {
            var payments = await _paymentService.GetPaymentsByUserAsync(userId);
            return Ok(payments);
        }

        /// <summary>
        /// Yeni ödeme oluşturur
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PaymentDTO>> Create([FromBody] CreatePaymentDTO dto)
        {
            var payment = await _paymentService.CreatePaymentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
        }

        /// <summary>
        /// Ödeme siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _paymentService.DeletePaymentAsync(id);

            if (!result)
                return NotFound(new { message = $"Payment with id {id} not found" });

            return NoContent();
        }
    }
}