using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NextGenRobotics.Models
{
    public class Cart
    {

        [Key]
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }

        public virtual Product Product { get; set; }

        public virtual User User { get; set; }
    }
}