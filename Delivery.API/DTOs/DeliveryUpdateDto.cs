using System.ComponentModel.DataAnnotations;

namespace Delivery_API.DTOs.Delivery
{
    public class DeliveryUpdateDto
    {
        [MaxLength(100)]
        public string Name { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public string Licencia { get; set; }
        public bool Disponible { get; set; }
        public string Vehiculo { get; set; }
        public string Placa { get; set; }
    }
}