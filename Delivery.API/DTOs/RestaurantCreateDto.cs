using System.ComponentModel.DataAnnotations;

namespace Delivery_API.DTOs.Restaurant
{
    public class RestaurantCreateDto
    {
        [Required]
        [MaxLength(150)]
        public string CommercialName { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        public string Category { get; set; }
        public bool State { get; set; } = true;
    }
}