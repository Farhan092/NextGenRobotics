using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextGenRobotics.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Display(Name = "Order Number")]
        public int OrderNo { get; set; }
        public int UserId { get; set; }

        public int PaymentId { get; set; }

        [Required]
        [Display(Name = "Order Date")]
        public Nullable<DateTime> OrderDate { get; set; }


        [Required]
        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Order Status")]
        public string Status { get; set; } // Pending, Confirmed, Canceled

        //[Required]
        [StringLength(500)]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }

        // Navigation properties
        public virtual User User { get; set; }

        

        public virtual Payment Payment { get; set; }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }



    }
}