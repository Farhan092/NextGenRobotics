using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextGenRobotics.ViewModels
{
    public class OrderSummaryViewModel
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }

        public int TotalCategories { get; set; }


        public List<OrderViewModel> Orders { get; set; }
    }
}