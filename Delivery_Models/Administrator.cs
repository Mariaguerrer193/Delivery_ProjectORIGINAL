using System;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Models
{
    public class Administrator : Person
    {
        [Required]
        [MaxLength(100)]
        public string Branch { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; }

        public double Salary { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(255)]
        public string AdminNotes { get; set; }

        public Administrator() { }

        public Administrator(string name, DateTime dateBirth, string address, string email, string phone, bool gender, string passwordHash, string branch, string role, double salary, DateTime hireDate, string adminNotes)
            : base(name, dateBirth, address, email, phone, gender, passwordHash)
        {
            Branch = branch;
            Role = role;
            Salary = salary;
            HireDate = hireDate;
            AdminNotes = adminNotes;
        }
    }
}