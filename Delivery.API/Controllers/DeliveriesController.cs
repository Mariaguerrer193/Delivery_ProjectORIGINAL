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
    public class DeliveriesController : ControllerBase
    {
        private readonly DeliveryAPIContext _context;

        public DeliveriesController(DeliveryAPIContext context)
        {
            _context = context;
        }

        // GET: api/Deliveries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Delivery_Models.Delivery>>> GetDeliveries()
        {
            // Ahora sí funciona: Delivery.Vehicle es una navegación real a la tabla Vehicles
            return await _context.Deliveries
                .Include(d => d.Orders)
                .Include(d => d.Vehicle)
                .ToListAsync();
        }

        // GET: api/Deliveries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Delivery_Models.Delivery>> GetDelivery(int id)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Orders)
                .Include(d => d.Vehicle)
                .Where(d => d.Id == id)
                .FirstOrDefaultAsync();

            if (delivery == null)
            {
                return NotFound();
            }

            return delivery;
        }

        // PUT: api/Deliveries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDelivery(int id, Delivery_Models.Delivery delivery)
        {
            if (id != delivery.Id)
            {
                return BadRequest();
            }

            _context.Entry(delivery).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeliveryExists(id))
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

        // POST: api/Deliveries
        [HttpPost]
        public async Task<ActionResult<Delivery_Models.Delivery>> PostDelivery(Delivery_Models.Delivery delivery)
        {
            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDelivery", new { id = delivery.Id }, delivery);
        }

        // DELETE: api/Deliveries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDelivery(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }

            _context.Deliveries.Remove(delivery);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeliveryExists(int id)
        {
            return _context.Deliveries.Any(e => e.Id == id);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableDeliveries()
        {
            var deliveries = await _context.Deliveries
                .Where(d => d.IsAvailable)
                .Include(d => d.Vehicle)
                .ToListAsync();
            return Ok(deliveries);
        }

        [HttpPut("{id}/available")]
        public async Task<IActionResult> SetDeliveryAvailable(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
                return NotFound("Delivery no encontrado");

            delivery.IsAvailable = true;
            await _context.SaveChangesAsync();

            return Ok(new { deliveryId = id, isAvailable = true });
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetDeliveryHistory(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
                return NotFound("Delivery no encontrado");

            var orders = await _context.Orders
                .Where(o => o.DeliveryId == id)
                .Include(o => o.Client)
                .Include(o => o.DetailOrders)
                    .ThenInclude(d => d.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(new
            {
                delivery = delivery.Name,
                totalDeliveries = orders.Count,
                completed = orders.Count(o => o.Status == "Entregado"),
                history = orders
            });
        }
    }
}