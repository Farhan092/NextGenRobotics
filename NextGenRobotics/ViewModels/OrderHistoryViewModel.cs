using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextGenRobotics.ViewModels
{
    public class OrderHistoryViewModel
    {
        public int OrderNo { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public double TotalAmount { get; set; }
        public string PaymentMode { get; set; }
        public string CardNo { get; set; }
        public string Status { get; set; }
    }
}