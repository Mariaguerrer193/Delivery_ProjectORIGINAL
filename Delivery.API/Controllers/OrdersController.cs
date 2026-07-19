using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery.API.Data;
using Delivery_Models;
//using Delivery.API.DTOs.Order;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DeliveryAPIContext _context;

        public OrdersController(DeliveryAPIContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Delivery)
                .Include(o => o.DetailOrders)
                .ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Client)
                    .ThenInclude(c => c.PayMethods)
                .Include(o => o.Delivery)
                .Include(o => o.DetailOrders)
                .ThenInclude(d => d.Product)
                .Where(o => o.Id == id)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }


        public class CheckoutItem
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        public class CheckoutRequest
        {
            public int ClientId { get; set; }
            public List<CheckoutItem> Items { get; set; }
            public string PayMethodType { get; set; }
            public string? CouponCode { get; set; }
            public double? LatCliente { get; set; }
            public double? LngCliente { get; set; }
        }


        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
                return BadRequest("El carrito está vacío");

            var order = new Order(DateTime.UtcNow, request.ClientId);
            order.LatCliente = request.LatCliente;
            order.LngCliente = request.LngCliente;
            order.ConfirmationCode = new Random().Next(1000, 9999).ToString();

            foreach (var item in request.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest($"El producto con id {item.ProductId} no existe");

                var detail = new DetailOrder(0, item.ProductId, item.Quantity, product.Price);
                order.DetailOrders.Add(detail);
            }

            if (!string.IsNullOrEmpty(request.CouponCode))
            {
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code == request.CouponCode && c.IsActive);
                if (coupon != null)
                    order.CouponId = coupon.Id;
            }

            // 🎯 REPARACIÓN MAESTRA: Guardamos el método de pago en la tabla PayMethods vinculada al cliente
            if (!string.IsNullOrEmpty(request.PayMethodType))
            {
                // Limpiamos métodos viejos para que el tracking lea el último usado en este pedido
                var oldMethods = _context.PayMethods.Where(p => p.ClientId == request.ClientId);
                _context.PayMethods.RemoveRange(oldMethods);

                // Insertamos el método actual (Efectivo, Tarjeta o Transferencia)
                var newPayMethod = new PayMethod(request.PayMethodType, "Checkout", request.ClientId);
                _context.PayMethods.Add(newPayMethod);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { orderId = order.Id });
        }


        [HttpGet("Client/{clientId}")]
        public async Task<IActionResult> GetOrdersByClient(int clientId)
        {
            var orders = await _context.Orders
                .Where(o => o.ClientId == clientId)
                .Include(o => o.DetailOrders)
                    .ThenInclude(d => d.Product)
                        .ThenInclude(p => p.Restaurant)
                .Include(o => o.Delivery)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var result = orders.Select(o => new
            {
                id = o.Id,
                status = o.Status,
                date = o.OrderDate.ToString("dd/MM/yyyy"),
                total = o.DetailOrders.Sum(d => d.Quantity * d.Price),
                restaurantId = o.DetailOrders.FirstOrDefault()?.Product?.RestaurantId,
                restaurantName = o.DetailOrders
                    .FirstOrDefault()?.Product?.Restaurant?.CommercialName ?? "Restaurante",
                deliveryId = o.DeliveryId,
                deliveryName = o.Delivery != null ? o.Delivery.Name : null,

                // --- NUEVO: Lista de productos incluidos en esta orden ---
                products = o.DetailOrders
                    .Where(d => d.Product != null)
                    .Select(d => new
                    {
                        productId = d.ProductId,
                        productName = d.Product!.Name
                    }).ToList()
            });

            return Ok(result);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }


        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Pedido no encontrado");

            var validStatus = new[] { "Pendiente", "En preparación", "En camino", "Entregado", "Cancelado" };
            if (!validStatus.Contains(status))
                return BadRequest("Estado inválido. Estados válidos: Pendiente, En preparación, En camino, Entregado, Cancelado");

            order.Status = status;

            // Si se entrega, liberar al delivery
            if (status == "Entregado" && order.DeliveryId.HasValue)
            {
                var delivery = await _context.Deliveries.FindAsync(order.DeliveryId.Value);
                if (delivery != null)
                    delivery.IsAvailable = true;
            }

            await _context.SaveChangesAsync();
            return Ok(new { orderId = id, status = order.Status });
        }


        [HttpGet("{id}/total")]
        public async Task<IActionResult> GetOrderTotal(int id)
        {
            var order = await _context.Orders
                .Include(o => o.DetailOrders)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound("Pedido no encontrado");

            var total = order.DetailOrders.Sum(d => d.Quantity * d.Price);
            return Ok(new { orderId = id, total = total, details = order.DetailOrders });
        }

        [HttpPost("{orderId}/assign-delivery/{deliveryId}")]
        public async Task<IActionResult> AssignDelivery(int orderId, int deliveryId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound("Pedido no encontrado");

            if (order.Status != "Pendiente" && order.Status != "En preparación")
                return BadRequest("Solo se puede asignar delivery a pedidos pendientes o en preparación");

            var delivery = await _context.Deliveries.FindAsync(deliveryId);
            if (delivery == null)
                return NotFound("Delivery no encontrado");

            if (!delivery.IsAvailable)
                return BadRequest("El delivery no está disponible");

            order.DeliveryId = deliveryId;
            order.Status = "En camino";
            delivery.IsAvailable = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                orderId = order.Id,
                deliveryId = delivery.Id,
                deliveryName = delivery.Name,
                status = order.Status
            });
        }

        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetOrdersByStatus(string status)
        {
            var validStatus = new[] { "Pendiente", "En preparación", "En camino", "Entregado", "Cancelado" };
            if (!validStatus.Contains(status))
                return BadRequest("Estado inválido");

            var orders = await _context.Orders
                .Where(o => o.Status == status)
                .Include(o => o.Client)
                    .ThenInclude(c => c.PayMethods)
                .Include(o => o.Delivery)
                .Include(o => o.DetailOrders)
                    .ThenInclude(d => d.Product)
                        .ThenInclude(p => p.Restaurant) // <-- ESTA LÍNEA ASEGURA QUE PASEN LAS COORDENADAS
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders);
        }
    }
}