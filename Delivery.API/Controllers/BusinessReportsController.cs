using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery.API.Data;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessReportsController : ControllerBase
    {
        private readonly DeliveryAPIContext _context;

        public BusinessReportsController(DeliveryAPIContext context)
        {
            _context = context;
        }

        // 1. PEDIDOS DEL DÍA (CORREGIDO PARA POSTGRESQL UTC)
        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyOrders()
        {
            // Creamos el inicio del día de hoy y mañana en formato UTC puro
            var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
            var tomorrow = DateTime.SpecifyKind(today.AddDays(1), DateTimeKind.Utc);

            var orders = await _context.Orders
                .Where(o => o.OrderDate >= today && o.OrderDate < tomorrow)
                .CountAsync();

            var total = await _context.Orders
                .Where(o => o.OrderDate >= today && o.OrderDate < tomorrow)
                .SelectMany(o => o.DetailOrders)
                .SumAsync(d => d.Quantity * d.Price);

            return Ok(new
            {
                date = today,
                orders = orders,
                total = total,
                message = orders == 0 ? "No hay pedidos hoy" : ""
            });
        }
        // 2. TOP DELIVERYS
        [HttpGet("top-deliveries")]
        public async Task<IActionResult> GetTopDeliveries()
        {
            var top = await _context.Deliveries
                .Select(d => new
                {
                    Id = d.Id,
                    Name = d.Name,
                    TotalOrders = d.Orders.Count,
                    CompletedOrders = d.Orders.Count(o => o.Status == "Entregado")
                })
                .OrderByDescending(d => d.CompletedOrders)
                .Take(10)
                .ToListAsync();

            return Ok(top);
        }

        // 3. PRODUCTOS MÁS VENDIDOS
        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts()
        {
            var top = await _context.DetailOrders
                .GroupBy(d => d.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product.Name,
                    TotalSold = g.Sum(d => d.Quantity),
                    TotalRevenue = g.Sum(d => d.Quantity * d.Price)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(10)
                .ToListAsync();

            return Ok(top);
        }

        // 4. ESTADÍSTICAS GENERALES (CORREGIDO PARA POSTGRESQL UTC)
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var totalClients = await _context.Clients.CountAsync();
            var totalDeliveries = await _context.Deliveries.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalRestaurants = await _context.Restaurants.CountAsync();

            // Aseguramos que la fecha de comparación sea UTC
            var todayUtc = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
            var todayOrders = await _context.Orders
                .CountAsync(o => o.OrderDate >= todayUtc);

            return Ok(new
            {
                totalClients,
                totalDeliveries,
                totalOrders,
                totalProducts,
                totalRestaurants,
                todayOrders,
                message = "Estadísticas generales"
            });
        }
    }
}