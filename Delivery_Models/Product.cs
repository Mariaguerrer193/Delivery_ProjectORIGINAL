using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery_Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public double Price { get; set; }

        public int Stock { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }

        [MaxLength(100)]
        public string Category { get; set; }

        public bool IsAvailable { get; set; } = true;

        [MaxLength(300)]
        public string PhotoUrl { get; set; } // <-- NUEVO: Campo para la imagen

        public Product() { }

        public Product(string name, string description, double price, int stock, int restaurantId, string category, string photoUrl)
        {
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            RestaurantId = restaurantId;
            Category = category;
            PhotoUrl = photoUrl;
        }
    }
}