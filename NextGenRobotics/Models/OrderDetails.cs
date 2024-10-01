using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NextGenRobotics.Models
{
    public class OrderDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailsId { get; set; }

        [Required]
        public int OrderId { get; set; } // Foreign key to the Order

        [Required]
        public int ProductId { get; set; } // Foreign key to the Product

        public int TotalPrice { get; set; } 

        [Required]
        public int Quantity { get; set; } // Number of items for the product

        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}