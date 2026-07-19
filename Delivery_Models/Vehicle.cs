using System;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required]
        [MaxLength(50)]
        public string Brand { get; set; }

        [Required]
        [MaxLength(50)]
        public string Model { get; set; }

        [Required]
        [MaxLength(20)]
        public string Plate { get; set; }

        [MaxLength(100)]
        public string Color { get; set; }

        public int Year { get; set; }

        public bool IsAvailable { get; set; } = true;

        public Vehicle() { }

        public Vehicle(string type, string brand, string model, string plate, string color, int year)
        {
            Type = type;
            Brand = brand;
            Model = model;
            Plate = plate;
            Color = color;
            Year = year;
        }
    }
}