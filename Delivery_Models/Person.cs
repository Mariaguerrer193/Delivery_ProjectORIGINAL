using System;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime DateBirth { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; }

        [Required]
        public bool Gender { get; set; }

        [Required]
        public string PasswordHash { get; set; }


        public bool IsActive { get; set; } = true;

        

        public Person() { }

        public Person(string name, DateTime dateBirth, string address, string email, string phone, bool gender, string passwordHash)
        {
            Name = name;
            DateBirth = dateBirth;
            Address = address;
            Email = email;
            Phone = phone;
            Gender = gender;
            PasswordHash = passwordHash;
        }

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }
    }
}


