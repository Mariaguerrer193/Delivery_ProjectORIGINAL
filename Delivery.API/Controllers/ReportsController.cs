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
    public class ReportsController : ControllerBase
    {
        private readonly DeliveryAPIContext _context;

        public ReportsController(DeliveryAPIContext context)
        {
            _context = context;
        }

        // GET: api/Reports (Admin ve todas las quejas pendientes del sistema)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> GetReports()
        {
            return await _context.Reports
                .Include(r => r.SourcePerson)
                .Include(r => r.Restaurant)
                .Include(r => r.Delivery)
                .Include(r => r.Product)
                .Include(r => r.Client)
                .Where(r => !r.IsResolved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // POST: api/Reports (Cualquier entidad envía una queja)
        [HttpPost]
        public async Task<IActionResult> PostReport(Report report)
        {
            // 1. Validar que se reporte exactamente a una entidad
            int targetsCount = 0;
            if (report.RestaurantId.HasValue) targetsCount++;
            if (report.DeliveryId.HasValue) targetsCount++;
            if (report.ProductId.HasValue) targetsCount++;
            if (report.ClientId.HasValue) targetsCount++;

            if (targetsCount != 1)
            {
                return BadRequest("El reporte debe ser dirigido exactamente a un Restaurante, un Delivery, un Producto o un Cliente.");
            }

            // 2. Validación cruzada de elegibilidad basada en órdenes compartidas "Entregadas"
            bool isEligible = false;

            // Caso A: Un Cliente reporta algo
            if (await _context.Clients.AnyAsync(c => c.Id == report.SourcePersonId))
            {
                var baseQuery = _context.Orders.Where(o => o.ClientId == report.SourcePersonId && o.Status == "Entregado");

                if (report.RestaurantId.HasValue)
                {
                    // Buscamos si el restaurante está en alguno de los productos de los detalles de sus órdenes
                    isEligible = await baseQuery.AnyAsync(o => o.DetailOrders.Any(d => d.Product != null && d.Product.RestaurantId == report.RestaurantId));
                }
                else if (report.DeliveryId.HasValue)
                {
                    isEligible = await baseQuery.AnyAsync(o => o.DeliveryId == report.DeliveryId);
                }
                else if (report.ProductId.HasValue)
                {
                    isEligible = await _context.DetailOrders.AnyAsync(d => d.ProductId == report.ProductId &&
                        _context.Orders.Any(o => o.Id == d.OrderId && o.ClientId == report.SourcePersonId && o.Status == "Entregado"));
                }
            }
            // Caso B: Un Repartidor reporta a un Cliente o Restaurante
            else if (await _context.Deliveries.AnyAsync(d => d.Id == report.SourcePersonId))
            {
                var baseQuery = _context.Orders.Where(o => o.DeliveryId == report.SourcePersonId && o.Status == "Entregado");

                if (report.ClientId.HasValue)
                {
                    isEligible = await baseQuery.AnyAsync(o => o.ClientId == report.ClientId);
                }
                else if (report.RestaurantId.HasValue)
                {
                    // El repartidor es elegible si la orden entregada contenía productos de ese restaurante
                    isEligible = await baseQuery.AnyAsync(o => o.DetailOrders.Any(d => d.Product != null && d.Product.RestaurantId == report.RestaurantId));
                }
            }
            // Caso C: Un Restaurante reporta a un Cliente o Repartidor
            else if (await _context.Restaurants.AnyAsync(r => r.Id == report.SourcePersonId))
            {
                // Filtramos las órdenes entregadas que tengan al menos un producto del restaurante que reporta
                var baseQuery = _context.Orders
                    .Where(o => o.Status == "Entregado" && o.DetailOrders.Any(d => d.Product != null && d.Product.RestaurantId == report.SourcePersonId));

                if (report.ClientId.HasValue)
                {
                    isEligible = await baseQuery.AnyAsync(o => o.ClientId == report.ClientId);
                }
                else if (report.DeliveryId.HasValue)
                {
                    isEligible = await baseQuery.AnyAsync(o => o.DeliveryId == report.DeliveryId);
                }
            }

            if (!isEligible)
            {
                return BadRequest("No se puede generar el reporte. No compartes un historial de pedido entregado con esa entidad.");
            }

            report.CreatedAt = DateTime.UtcNow;
            report.IsResolved = false;

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return Ok(new { reportId = report.Id, message = "Reporte procesado correctamente por el sistema." });
        }

        // PUT: api/Reports/5/resolve (Admin resuelve la queja)
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveReport(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound("Reporte no encontrado.");

            report.IsResolved = true;
            await _context.SaveChangesAsync();

            return Ok(new { reportId = id, message = "El reporte ha sido marcado como resuelto." });
        }
    }
}