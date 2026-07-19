using System.ComponentModel.DataAnnotations;

namespace Delivery_API.DTOs.Auth
{
    public class RegisterDto
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
        public string Role { get; set; }
    }
}