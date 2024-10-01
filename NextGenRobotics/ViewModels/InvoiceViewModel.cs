using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextGenRobotics.ViewModels
{
    public class InvoiceViewModel
    {
        public int OrderNo { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public double TotalAmount { get; set; }

        public string Address { get; set; }
    }
}