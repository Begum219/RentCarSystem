using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;
using System.Security.Claims;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentGatewayService _paymentGatewayService;

        public PaymentController(IPaymentGatewayService paymentGatewayService)
        {
            _paymentGatewayService = paymentGatewayService;
        }

        /// <summary>
        /// Ödeme işle - Tam (Kart bilgileri ile)
        /// </summary>
        [HttpPost("process")]
        [Authorize]
        public async Task<ActionResult<PaymentResult>> ProcessPayment([FromBody] PaymentRequest request)
        {
            var result = await _paymentGatewayService.ProcessPaymentAsync(request);

            if (result.Success)
                return Ok(new { success = true, data = result });
            else
                return BadRequest(new { success = false, data = result });
        }

        /// <summary>
        /// Ödeme işle - Basit (Otomatik test kartı)
        /// </summary>
        [HttpPost("process-simple")]
        [Authorize]
        public async Task<ActionResult<PaymentResult>> ProcessSimplePayment([FromBody] SimplePaymentRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _paymentGatewayService.ProcessSimplePaymentAsync(userId, request);

            if (result.Success)
                return Ok(new { success = true, data = result });
            else
                return BadRequest(new { success = false, data = result });
        }
    }
}