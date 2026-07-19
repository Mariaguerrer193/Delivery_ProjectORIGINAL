namespace Delivery_API.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string Category { get; set; }
        public bool IsAvailable { get; set; }
    }
}