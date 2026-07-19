using System.ComponentModel.DataAnnotations;

namespace Delivery_API.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Mail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}