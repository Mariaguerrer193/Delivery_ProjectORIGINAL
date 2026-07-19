using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Models
{
    public class Client : Person
    {
        [MaxLength(200)]
        public string MainAddress { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();

        public List<PayMethod> PayMethods { get; set; } = new List<PayMethod>();

        public Client() { }

        public Client(string name, DateTime dateBirth, string address, string email, string phone, bool gender, string passwordHash, string mainAddress, double latitude, double longitude)
            : base(name, dateBirth, address, email, phone, gender, passwordHash)
        {
            MainAddress = mainAddress;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}