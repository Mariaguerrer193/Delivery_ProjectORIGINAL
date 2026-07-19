using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delivery_Models
{
    
        public class Report
        {
        [Key]
        public int Id { get; set; }

        // Quién emite el reporte (puede ser un Cliente, Repartidor o Restaurante)
        [Required]
        public int SourcePersonId { get; set; }

        [ForeignKey("SourcePersonId")]
        public Person? SourcePerson { get; set; }

        // --- A QUIÉN O QUÉ SE REPORTA (Exactamente uno de estos cuatro debe tener valor) ---

        public int? RestaurantId { get; set; }
        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }

        public int? DeliveryId { get; set; }
        [ForeignKey("DeliveryId")]
        public Delivery? Delivery { get; set; }

        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        // NUEVO: Permite que el Restaurante o el Delivery reporten a un Cliente
        public int? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsResolved { get; set; } = false;

        public Report() { }
    }
    
}
