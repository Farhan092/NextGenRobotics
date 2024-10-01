using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NextGenRobotics.ViewModels
{
    public class ProductDetailsVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }

        [Required]
        public int UnitInStock { get; set; }
        public string StockStatus { get; set; }
        public string Description { get; set; }
        public string PicturePath { get; set; }
        public string CategoryName { get; set; }
    }
}