namespace Delivery_API.DTOs.Delivery
{
    public class DeliveryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Licencia { get; set; }
        public bool Disponible { get; set; }
        public string Vehiculo { get; set; }
        public string Placa { get; set; }
        public int OrdersCount { get; set; }
    }
}