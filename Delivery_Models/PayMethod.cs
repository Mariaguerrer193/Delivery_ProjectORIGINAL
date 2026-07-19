using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery_Models
{
    public class PayMethod
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [MaxLength(100)]
        public string Provider { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        public PayMethod() { }

        public PayMethod(string type, string provider, int clientId)
        {
            Type = type;
            Provider = provider;
            ClientId = clientId;
        }
    }
}