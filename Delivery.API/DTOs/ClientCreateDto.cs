using System.ComponentModel.DataAnnotations;

namespace Delivery_API.DTOs.Client
{
    public class ClientCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Mail { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public DateTime DateBirth { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public bool Gender { get; set; }
        public string DireccionPrincipal { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
}