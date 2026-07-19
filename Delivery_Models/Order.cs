using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery_Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public int ClientId { get; set; }

        // Se mantiene como string (no enum) para no romper el OrdersController que ya funciona
        public string Status { get; set; } = "Pendiente";

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        public int? DeliveryId { get; set; }

        [ForeignKey("DeliveryId")]
        public Delivery? Delivery { get; set; }

        // --- NUEVO: cupón aplicado (opcional) ---
        public int? CouponId { get; set; }

        [ForeignKey("CouponId")]
        public Coupon? Coupon { get; set; }

        // --- NUEVO: tracking en tiempo real para el mapa ---
        public double? LatRepartidor { get; set; }
        public double? LngRepartidor { get; set; }
        public double? LatCliente { get; set; }
        public double? LngCliente { get; set; }

        // --- NUEVO: código para confirmar la entrega en persona ---
        [MaxLength(10)]
        public string? ConfirmationCode { get; set; }

        public ICollection<DetailOrder> DetailOrders { get; set; }

        public Order()
        {
            DetailOrders = new HashSet<DetailOrder>();
        }

        public Order(DateTime orderDate, int clientId)
        {
            OrderDate = orderDate;
            ClientId = clientId;
            DetailOrders = new HashSet<DetailOrder>();
        }


    }
}