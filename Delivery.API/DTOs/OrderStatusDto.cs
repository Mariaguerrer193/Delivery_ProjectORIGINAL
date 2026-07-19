using System.ComponentModel.DataAnnotations;

namespace Delivery.API.DTOs.Order
{
    public class OrderStatusDto
    {
        [Required]
        public string Status { get; set; }
    }
}