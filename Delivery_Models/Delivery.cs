using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Delivery_Models
{
    public class Delivery : Person
    {
        [MaxLength(50)]
        public string License { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsApproved { get; set; } = false; // el Admin lo aprueba antes de operar

        // --- Antes eran strings sueltos (Vehicle, Plate) ---
        // --- Ahora es una relación real con la tabla Vehicles ---
        public int? VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle? Vehicle { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();

        public Delivery() { }

        public Delivery(string name, DateTime dateBirth, string address, string email, string phone, bool gender, string passwordHash, string license)
            : base(name, dateBirth, address, email, phone, gender, passwordHash)
        {
            License = license;
        }
    }
}