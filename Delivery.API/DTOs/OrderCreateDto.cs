using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delivery.API.DTOs.Order
{
    public class OrderCreateDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe tener al menos un producto")]
        public List<OrderItemDto> Items { get; set; }
    }

    public class OrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }
    }
}