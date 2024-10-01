using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NextGenRobotics.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [Required]
        [DisplayName("Card Owner")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Card owner name cannot contain numbers or special characters")]
        [StringLength(50, ErrorMessage = "Card owner name cannot exceed 50 characters")]
        public string UserName { get; set; }
        [Required]
        [Range(100000000000000, 999999999999999, ErrorMessage = "Card number must be a 15-digit number")]
        [RegularExpression(@"^\d{15}$", ErrorMessage = "Card number must be 15 digits")]
        [DisplayName("Card Number")]
        public string CardNo { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Expire Date")]

        public string ExpDate { get; set; }
        [Required]
        [Range(100, 999, ErrorMessage = "CVV must be a 3-digit number")]
        public int Cvv { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Address cannot exceed 50 characters")]
        public string Address { get; set; }
        [Required]
        public string PaymentMode { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

    }
}