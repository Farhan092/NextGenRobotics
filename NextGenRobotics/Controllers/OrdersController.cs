using NextGenRobotics.Context;
using NextGenRobotics.Models;
using NextGenRobotics.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace NextGenRobotics.Controllers
{
    public class OrdersController : Controller
    {
        private AspRoboDB dbContext;

        // Constructor to initialize the database context
        public OrdersController()
        {
            dbContext = new AspRoboDB();
        }

        // Action to display all orders along with the order summary
        public ActionResult Index()
        {
            if (Session["username"] != null && Session["Role"].ToString().Equals("Admin"))
            {

            // Retrieve order data from the database and map to OrderViewModel
            var orders = dbContext.Orders.Select(order => new OrderViewModel
            {
                OrderId = order.OrderId,
                OrderNo = order.OrderNo.ToString(),
                OrderDate = (DateTime)order.OrderDate,
                UserName = order.User.Name,
                UserPhoneNumber = order.User.PhoneNumber,
                PaymentType = order.Payment.PaymentMode,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                Products = order.OrderDetails.Select(p => new OrderProductViewModel
                {
                    ProductName = p.Product.Name,
                    Quantity = p.Quantity,
                    TotalPrice = p.TotalPrice
                }).ToList(),
            }).ToList();


            var totalProducts = dbContext.Products.Count();

            var totalCategory = dbContext.Categories.Count();

            // Calculate total sales and total orders
            var orderSummary = new OrderSummaryViewModel
            {
                //TotalSales = orders.Sum(o => o.TotalAmount),
                TotalSales = orders.Where(o => o.Status == "Confirmed").Sum(o => o.TotalAmount),
                // Total sales (sum of all order amounts)
                TotalOrders = orders.Count,
                TotalProducts = dbContext.Products.Count(),
                TotalCategories = dbContext.Categories.Count(),

                Orders = orders  // The list of individual order view models
            };

            // Pass the summary data to the view
            return View(orderSummary);
        }
            return RedirectToAction("Login", "Users");
        }

        // Action to update the order status
        [HttpPost]
        public ActionResult UpdateStatus(int orderId, string status)
        {
            // Find the order by its ID
            var order = dbContext.Orders.SingleOrDefault(o => o.OrderId == orderId);

            if (order != null)
            {
                // Update the order status
                order.Status = status;
                dbContext.SaveChanges(); // Save the changes to the database
            }

            // Redirect back to the index (or you can use AJAX to update in place)
            return RedirectToAction("Index");
        }



        public ActionResult OrderSummary()
        {
            int userId = Convert.ToInt32(Session["id"]);

            // Fetch the current order and payment info for the logged-in user based on the orderId
            var invoices = (from order in dbContext.Orders
                            join orderDetail in dbContext.OrderDetails on order.OrderId equals orderDetail.OrderId
                            join product in dbContext.Products on orderDetail.ProductId equals product.ProductId
                            join payment in dbContext.Payments on order.PaymentId equals payment.PaymentId
                            where order.UserId == userId
                            select new OrderHistoryViewModel
                            {
                                OrderNo = order.OrderNo,
                                ProductName = product.Name,
                                UnitPrice = (double)product.UnitPrice,
                                Quantity = orderDetail.Quantity,
                                TotalPrice = orderDetail.TotalPrice,
                                TotalAmount = (double)order.TotalAmount,

                                // Add payment information and mask the card number
                                PaymentMode = payment.PaymentMode,
                                CardNo = "**" + payment.CardNo.Substring(payment.CardNo.Length - 4),

                                // Add order status
                                Status = order.Status
                            }).ToList();



            return View(invoices);
        }






























        // Dispose the database context to release resources
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
