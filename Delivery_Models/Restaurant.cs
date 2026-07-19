using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Models
{
    public class Restaurant : Person
    {
        [Required]
        [MaxLength(150)]
        public string CommercialName { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        public bool IsOpen { get; set; } = true;

        public bool IsApproved { get; set; } = false; // el Admin lo aprueba antes de operar


        [MaxLength(300)]
        public string? PhotoUrl { get; set; }   // <-- NUEVO

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public List<Product> Menu { get; set; } = new List<Product>();

        public Restaurant() { }

        public Restaurant(string name, DateTime dateBirth, string address, string email, string phone, bool gender, string passwordHash, string commercialName, string category)
            : base(name, dateBirth, address, email, phone, gender, passwordHash)
        {
            CommercialName = commercialName;
            Category = category;
        }
    }
}