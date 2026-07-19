using System.Collections.Generic;
using Delivery_API.DTOs.Product;

namespace Delivery_API.DTOs.Restaurant
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string CommercialName { get; set; }
        public string Address { get; set; }
        public string Category { get; set; }
        public bool State { get; set; }
        public int TotalProducts { get; set; }
        public List<ProductDto> Menu { get; set; }
    }
}