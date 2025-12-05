using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Tüm yorumları listeler
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ReviewDTO>>> GetAll()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        /// <summary>
        /// ID'ye göre yorum getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDTO>> GetById(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);

            if (review == null)
                return NotFound(new { message = $"Review with id {id} not found" });

            return Ok(review);
        }

        /// <summary>
        /// Araca göre yorumları listeler
        /// </summary>
        [HttpGet("vehicle/{vehicleId}")]
        public async Task<ActionResult<List<ReviewDTO>>> GetByVehicle(int vehicleId)
        {
            var reviews = await _reviewService.GetReviewsByVehicleAsync(vehicleId);
            return Ok(reviews);
        }

        /// <summary>
        /// Kullanıcıya göre yorumları listeler
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ReviewDTO>>> GetByUser(int userId)
        {
            var reviews = await _reviewService.GetReviewsByUserAsync(userId);
            return Ok(reviews);
        }

        /// <summary>
        /// Onaylı yorumları listeler
        /// </summary>
        [HttpGet("approved")]
        public async Task<ActionResult<List<ReviewDTO>>> GetApproved()
        {
            var reviews = await _reviewService.GetApprovedReviewsAsync();
            return Ok(reviews);
        }

        /// <summary>
        /// Onay bekleyen yorumları listeler
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ReviewDTO>>> GetPending()
        {
            var reviews = await _reviewService.GetPendingReviewsAsync();
            return Ok(reviews);
        }

        /// <summary>
        /// Yeni yorum oluşturur
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewDTO>> Create([FromBody] CreateReviewDTO dto)
        {
            var review = await _reviewService.CreateReviewAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        }

        /// <summary>
        /// Yorum günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize] 
        public async Task<ActionResult<ReviewDTO>> Update(int id, [FromBody] UpdateReviewDTO dto)
        {
            var review = await _reviewService.UpdateReviewAsync(id, dto);
            return Ok(review);
        }

        /// <summary>
        /// Yorumu onaylar
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Approve(int id)
        {
            var result = await _reviewService.ApproveReviewAsync(id);

            if (!result)
                return NotFound(new { message = $"Review with id {id} not found" });

            return Ok(new { message = "Review approved successfully" });
        }

        /// <summary>
        /// Yorum siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);

            if (!result)
                return NotFound(new { message = $"Review with id {id} not found" });

            return NoContent();
        }
    }
}