using Delivery.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly DeliveryAPIContext _context;

        public TrackingController(DeliveryAPIContext context)
        {
            _context = context;
        }

        public class UpdateLocationRequest
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }

        // El repartidor manda su ubicación cada pocos segundos mientras el pedido está en camino
        [HttpPut("{orderId}/ubicacion-repartidor")]
        public async Task<IActionResult> ActualizarUbicacionRepartidor(int orderId, [FromBody] UpdateLocationRequest request)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound("Pedido no encontrado");

            order.LatRepartidor = request.Lat;
            order.LngRepartidor = request.Lng;
            await _context.SaveChangesAsync();

            return Ok(new { orderId, lat = request.Lat, lng = request.Lng });
        }

        // Se llama una sola vez al crear el pedido, con la dirección de entrega ya geocodificada
        [HttpPut("{orderId}/ubicacion-cliente")]
        public async Task<IActionResult> ActualizarUbicacionCliente(int orderId, [FromBody] UpdateLocationRequest request)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound("Pedido no encontrado");

            order.LatCliente = request.Lat;
            order.LngCliente = request.Lng;
            await _context.SaveChangesAsync();

            return Ok(new { orderId, lat = request.Lat, lng = request.Lng });
        }



        // El cliente y el repartidor consultan esto para mover el marcador
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetUbicacion(int orderId)
        {
            // Traemos la orden y sus detalles básicos (solo los IDs)
            var order = await _context.Orders
                .Include(o => o.DetailOrders)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound("Pedido no encontrado");

            double? latRestaurante = null;
            double? lngRestaurante = null;

            // Extraemos el primer detalle de la lista
            var firstDetail = order.DetailOrders.FirstOrDefault();

            if (firstDetail != null)
            {
                // BUSQUEDA DIRECTA POR ID: Si el Include falló, esto busca directo en la tabla Products
                var product = await _context.Products.FindAsync(firstDetail.ProductId);

                if (product != null)
                {
                    // BUSQUEDA DIRECTA POR ID: Vamos directo a la tabla de restaurantes usando el ID plano
                    var restaurant = await _context.Restaurants.FindAsync(product.RestaurantId);

                    if (restaurant != null)
                    {
                        latRestaurante = restaurant.Latitude;
                        lngRestaurante = restaurant.Longitude;
                    }
                }
            }

            return Ok(new
            {
                orderId = order.Id,
                status = order.Status,
                latRepartidor = order.LatRepartidor,
                lngRepartidor = order.LngRepartidor,
                latCliente = order.LatCliente,
                lngCliente = order.LngCliente,
                latRestaurante = latRestaurante,
                lngRestaurante = lngRestaurante
            });
        }
    }
}