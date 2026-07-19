namespace Delivery_API.DTOs.Client
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string DireccionPrincipal { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public int TotalOrders { get; set; }
    }
}