using NextGenRobotics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextGenRobotics.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string ProductNames { get; set; } // Concatenated product names
        public string PaymentType { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public string ShippingAddress { get; set; }

        public List<OrderProductViewModel> Products { get; set; }
        public string ProductsList => string.Join(", ", Products.Select(p => p.ProductName));
    }

    public class OrderProductViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }





}