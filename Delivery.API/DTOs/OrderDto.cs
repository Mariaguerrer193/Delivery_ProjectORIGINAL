using System;
using System.Collections.Generic;

namespace Delivery_API.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime DateOrder { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int? DeliveryId { get; set; }
        public string DeliveryName { get; set; }
        public List<OrderDetailDto> Details { get; set; }
        public double TotalAmount { get; set; }
    }

    public class OrderDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Subtotal { get; set; }
    }
}