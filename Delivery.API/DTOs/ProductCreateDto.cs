using System.ComponentModel.DataAnnotations;

namespace Delivery_API.DTOs.Product
{
    public class ProductCreateDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public double Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        public string Category { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}