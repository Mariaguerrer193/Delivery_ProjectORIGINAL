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
    public class PayMethodsController : ControllerBase
    {
        private readonly DeliveryAPIContext _context;

        public PayMethodsController(DeliveryAPIContext context)
        {
            _context = context;
        }

        // GET: api/PayMethods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PayMethod>>> GetPayMethods()
        {
            return await _context.PayMethods
                .Include(p => p.Client)
                .ToListAsync();
        }

        // GET: api/PayMethods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PayMethod>> GetPayMethod(int id)
        {
            var payMethod = await _context.PayMethods
                .Include(p => p.Client)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (payMethod == null)
            {
                return NotFound();
            }

            return payMethod;
        }

        // PUT: api/PayMethods/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayMethod(int id, PayMethod payMethod)
        {
            if (id != payMethod.Id)
            {
                return BadRequest();
            }

            _context.Entry(payMethod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayMethodExists(id))
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

        // POST: api/PayMethods
        [HttpPost]
        public async Task<ActionResult<PayMethod>> PostPayMethod(PayMethod payMethod)
        {
            _context.PayMethods.Add(payMethod);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPayMethod", new { id = payMethod.Id }, payMethod);
        }

        // DELETE: api/PayMethods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayMethod(int id)
        {
            var payMethod = await _context.PayMethods.FindAsync(id);
            if (payMethod == null)
            {
                return NotFound();
            }

            _context.PayMethods.Remove(payMethod);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PayMethodExists(int id)
        {
            return _context.PayMethods.Any(e => e.Id == id);
        }
    }
}