using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery.API.Data;
using Delivery_Models;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsController : ControllerBase
    {
        private readonly DeliveryAPIContext _context;

        public DetailsController(DeliveryAPIContext context)
        {
            _context = context;
        }

        // Detalle de Administrator
        [HttpGet("Administrator/{id}")]
        public async Task<ActionResult<Administrator>> GetAdministratorDetails(int id)
        {
            var admin = await _context.Administrators.FindAsync(id);
            if (admin == null) return NotFound();
            return admin;
        }

        // Detalle de Client
        [HttpGet("Client/{id}")]
        public async Task<ActionResult<Client>> GetClientDetails(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Orders)
                .Include(c => c.PayMethods)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null) return NotFound();
            return client;
        }

        
        [HttpGet("Delivery/{id}")]
        public async Task<ActionResult<Delivery_Models.Delivery>> GetDeliveryDetails(int id)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Orders)
                .Include(d => d.Vehicle)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (delivery == null) return NotFound();
            return delivery;
        }
    }
}