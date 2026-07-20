using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery.API.Data;
using Delivery_Models;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly DeliveryAPIContext _context;

        public ReviewsController(DeliveryAPIContext context)
        {
            _context = context;
        }

        // GET: api/Reviews  (Admin ve todas)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            return await _context.Reviews
                .Include(r => r.Client)
                .Include(r => r.Product)
                .Where(r => !r.IsRemoved)
                .ToListAsync();
        }

        // GET: api/Reviews/restaurant/5  (reseñas de un restaurante)
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetReviewsByRestaurant(int restaurantId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Client)
                .Where(r => r.RestaurantId == restaurantId && !r.IsRemoved)
                .ToListAsync();

            return Ok(reviews);
        }

        // GET: api/Reviews/delivery/5  (reseñas de un repartidor)
        [HttpGet("delivery/{deliveryId}")]
        public async Task<IActionResult> GetReviewsByDelivery(int deliveryId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Client)
                .Where(r => r.DeliveryId == deliveryId && !r.IsRemoved)
                .ToListAsync();

            return Ok(reviews);
        }

        // GET: api/Reviews/product/5  (reseñas de un producto/plato)
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsByProduct(int productId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Client)
                .Where(r => r.ProductId == productId && !r.IsRemoved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(reviews);
        }

        // GET: api/Reviews/reported  (Admin ve solo las reportadas)
        [HttpGet("reported")]
        public async Task<IActionResult> GetReportedReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Client)
                .Where(r => r.IsReported && !r.IsRemoved)
                .ToListAsync();

            return Ok(reviews);
        }

        // POST: api/Reviews  (Cliente crea una reseña)
        [HttpPost]
        public async Task<ActionResult<Review>> PostReview(Review review)
        {
            int targetsCount = 0;
            if (review.RestaurantId.HasValue) targetsCount++;
            if (review.DeliveryId.HasValue) targetsCount++;
            if (review.ProductId.HasValue) targetsCount++;

            if (targetsCount != 1)
            {
                return BadRequest("La reseña debe ser exactamente para un Restaurante, un Delivery o un Producto.");
            }

            review.CreatedAt = DateTime.UtcNow;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReviews), new { id = review.Id }, review);
        }

        public class ReportRequest
        {
            public string Reason { get; set; } = string.Empty;
        }

        // PUT: api/Reviews/5/report  (Cliente reporta una reseña falsa)
        [HttpPut("{id}/report")]
        public async Task<IActionResult> ReportReview(int id, [FromBody] ReportRequest request)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound("Reseña no encontrada");

            review.IsReported = true;
            review.ReportReason = request.Reason;
            await _context.SaveChangesAsync();

            return Ok(new { reviewId = id, message = "Reseña reportada, un administrador la revisará" });
        }

        // DELETE: api/Reviews/5  (Admin elimina una reseña moderada)
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound("Reseña no encontrada");

            review.IsRemoved = true;
            await _context.SaveChangesAsync();

            return Ok(new { reviewId = id, message = "Reseña eliminada por moderación" });
        }

        // PUT: api/Reviews/5/dismiss  (Admin descarta el reporte)
        [HttpPut("{id}/dismiss")]
        public async Task<IActionResult> DismissReport(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound("Reseña no encontrada");

            review.IsReported = false;
            review.ReportReason = null;
            await _context.SaveChangesAsync();

            return Ok(new { reviewId = id, message = "Reporte descartado, la reseña se mantiene" });
        }


        public class UpdateReviewRequest
        {
            public int Rating { get; set; }
            public string? Comment { get; set; }
            public string? PhotoUrl { get; set; }
        }

        // PUT: api/Reviews/5  (Cliente edita su propia reseña)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewRequest request)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound("Reseña no encontrada");

            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.PhotoUrl = request.PhotoUrl;

            await _context.SaveChangesAsync();
            return Ok(new { reviewId = id, message = "Reseña actualizada correctamente" });
        }
    }
}