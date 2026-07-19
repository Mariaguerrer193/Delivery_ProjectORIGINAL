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
    public class CouponsController : ControllerBase
    {
        private readonly DeliveryAPIContext _context;

        public CouponsController(DeliveryAPIContext context)
        {
            _context = context;
        }

        // GET: api/Coupons  (Admin ve todos)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetCoupons()
        {
            return await _context.Coupons.ToListAsync();
        }

        // GET: api/Coupons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Coupon>> GetCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();
            return coupon;
        }

        // POST: api/Coupons  (Admin crea cupón)
        [HttpPost]
        public async Task<ActionResult<Coupon>> PostCoupon(Coupon coupon)
        {
            var existing = await _context.Coupons.AnyAsync(c => c.Code == coupon.Code);
            if (existing)
                return BadRequest("Ya existe un cupón con ese código");

            // 🎯 CORRECCIÓN QUIRÚRGICA: Aseguramos que la fecha ingresada sea tratada como UTC por PostgreSQL
            if (coupon.ExpirationDate != DateTime.MinValue)
            {
                coupon.ExpirationDate = DateTime.SpecifyKind(coupon.ExpirationDate, DateTimeKind.Utc);
            }

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCoupon", new { id = coupon.Id }, coupon);
        }

        // PUT: api/Coupons/5 (Admin edita parámetros del cupón)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCoupon(int id, Coupon coupon)
        {
            if (id != coupon.Id)
            {
                return BadRequest();
            }

            // 🎯 CORRECCIÓN POSTGRESQL: Forzamos que la fecha editada sea UTC
            if (coupon.ExpirationDate != DateTime.MinValue)
            {
                coupon.ExpirationDate = DateTime.SpecifyKind(coupon.ExpirationDate, DateTimeKind.Utc);
            }

            _context.Entry(coupon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 🎯 REPARACIÓN: Consulta LINQ directa al contexto para eliminar el error 'CouponExists'
                if (!_context.Coupons.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Coupons/5/toggle  (Admin activa/desactiva)
        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> ToggleCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound("Cupón no encontrado");

            coupon.IsActive = !coupon.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { couponId = id, isActive = coupon.IsActive });
        }

        // DELETE: api/Coupons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Coupons/validate  (Cliente intenta aplicar un cupón)
        public class ValidateCouponRequest
        {
            public string Code { get; set; }
            public double OrderTotal { get; set; }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateCoupon([FromBody] ValidateCouponRequest request)
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == request.Code);

            if (coupon == null)
                return NotFound("Cupón no encontrado");

            if (!coupon.IsActive)
                return BadRequest("Este cupón ya no está activo");

            if (coupon.ExpirationDate < DateTime.UtcNow)
                return BadRequest("Este cupón ya expiró");

            if (coupon.UsageLimit.HasValue && coupon.TimesUsed >= coupon.UsageLimit.Value)
                return BadRequest("Este cupón alcanzó su límite de usos");

            double discount = coupon.IsPercentage
                ? request.OrderTotal * (coupon.Value / 100.0)
                : coupon.Value;

            double finalTotal = Math.Max(0, request.OrderTotal - discount);

            return Ok(new
            {
                valid = true,
                couponId = coupon.Id,
                code = coupon.Code,
                discount = discount,
                originalTotal = request.OrderTotal,
                finalTotal = finalTotal
            });
        }

        // POST: api/Coupons/5/use  (se llama cuando el pedido se confirma con ese cupón)
        [HttpPost("{id}/use")]
        public async Task<IActionResult> UseCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound("Cupón no encontrado");

            coupon.TimesUsed++;
            await _context.SaveChangesAsync();

            return Ok(new { couponId = id, timesUsed = coupon.TimesUsed });
        }
    }
}