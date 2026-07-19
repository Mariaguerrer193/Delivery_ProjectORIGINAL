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
    public class ClientsController : ControllerBase
    {
        private readonly DeliveryAPIContext _context;

        public ClientsController(DeliveryAPIContext context)
        {
            _context = context;
        }

        // GET: api/Clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients
                .Include(c => c.Orders)
                .Include(c => c.PayMethods)
                .ToListAsync();
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Orders)
                .Include(c => c.PayMethods)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        // PUT: api/Clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
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

        // POST: api/Clients
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = client.Id }, client);
        }

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }



        [HttpGet("{id}/orders")]
        public async Task<IActionResult> GetClientOrders(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound("Cliente no encontrado");

            var orders = await _context.Orders
                .Where(o => o.ClientId == id)
                .Include(o => o.Delivery)
                .Include(o => o.DetailOrders)
                    .ThenInclude(d => d.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(new
            {
                client = client.Name,
                totalOrders = orders.Count,
                orders = orders
            });
        }


    }
}