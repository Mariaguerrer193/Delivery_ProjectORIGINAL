using System;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Code { get; set; } // ej: "DESCUENTO10"

        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        public bool IsPercentage { get; set; } // true = %, false = monto fijo

        [Required]
        public double Value { get; set; } // 10 (%) o 5.00 (monto fijo)

        [Required]
        public DateTime ExpirationDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int? UsageLimit { get; set; } // null = ilimitado
        public int TimesUsed { get; set; } = 0;

        public Coupon() { }

        public Coupon(string code, string description, bool isPercentage, double value, DateTime expirationDate, int? usageLimit)
        {
            Code = code;
            Description = description;
            IsPercentage = isPercentage;
            Value = value;
            ExpirationDate = expirationDate;
            UsageLimit = usageLimit;
        }
    }
}