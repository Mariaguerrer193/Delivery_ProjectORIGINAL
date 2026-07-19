using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery_Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        // Solo UNO de los tres debe tener valor: Restaurante, Delivery o Producto
        public int? RestaurantId { get; set; }

        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }

        public int? DeliveryId { get; set; }

        [ForeignKey("DeliveryId")]
        public Delivery? Delivery { get; set; }

        public int? ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        [MaxLength(300)]
        public string? PhotoUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Moderación (Admin) ---
        public bool IsReported { get; set; } = false;

        [MaxLength(300)]
        public string? ReportReason { get; set; }

        public bool IsRemoved { get; set; } = false; // eliminada por el Admin

        public Review() { }
    }
}