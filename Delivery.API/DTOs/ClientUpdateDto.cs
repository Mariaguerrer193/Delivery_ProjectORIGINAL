using System.ComponentModel.DataAnnotations;

namespace Delivery_API.DTOs.Client
{
    public class ClientUpdateDto
    {
        [MaxLength(100)]
        public string Name { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public string DireccionPrincipal { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
}