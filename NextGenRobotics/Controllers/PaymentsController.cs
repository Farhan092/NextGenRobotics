using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NextGenRobotics.Context;
using NextGenRobotics.Models;
using NextGenRobotics.ViewModels;
using Rotativa;


namespace NextGenRobotics.Controllers
{
    public class PaymentsController : Controller
    {
        private AspRoboDB db = new AspRoboDB();

        // GET: Payments
        public ActionResult Index()
        {
            return View(db.Payments.ToList());
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: Payments/Create
        public ActionResult Create()
        {
            if (Session["username"] != null && (Session["Role"].ToString().Equals("Customer") || Session["Role"].ToString().Equals("Admin")))
            {
                return View();
        }
            return RedirectToAction("Login", "Users");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PaymentId,UserName,CardNo,ExpDate,Cvv,Address,PaymentMode")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                int userId = Convert.ToInt32(Session["id"]);
                if (string.IsNullOrWhiteSpace(payment.Address))
                {
                    payment.Address = "Address not available";
                }
                //if (string.IsNullOrWhiteSpace(payment.UserName) && string.IsNullOrWhiteSpace(payment.CardNo) && string.IsNullOrWhiteSpace(payment.ExpDate) && payment.Cvv == 0)
                //{
                //    payment.UserName = "not available";
                //    payment.CardNo = "0000000000000000";
                //    payment.ExpDate = "0000-00-00";
                //    payment.Cvv = 000;
                //}

                if (string.IsNullOrWhiteSpace(payment.UserName))
                {
                    payment.UserName = "not available";
                }

                if (string.IsNullOrWhiteSpace(payment.CardNo))
                {
                    payment.CardNo = "0000000000000000"; // Use a default valid card number or handle this differently
                }

                if (string.IsNullOrWhiteSpace(payment.ExpDate))
                {
                    payment.ExpDate = "1970-01-01"; // Or some valid default date
                }

                if (payment.Cvv == 0)
                {
                    payment.Cvv = 000; // Set a default valid CVV
                }


                // Save the payment first to get its PaymentId
                db.Payments.Add(payment);
                db.SaveChanges(); // Save the payment to the database

                var cartItems = db.Carts.Where(c => c.UserId == userId).ToList();
                decimal grandTotal = cartItems.Sum(item => item.Quantity * item.Product.UnitPrice);
                int lastOrderNo = db.Orders.OrderByDescending(o => o.OrderNo).FirstOrDefault()?.OrderNo ?? 1100;
                // Create the Order after saving the payment
                Order order = new Order
                {
                    //OrderNo = new Random().Next(1000, 9999), // Example order number generation
                    OrderNo = lastOrderNo + 1,
                    UserId = userId, // Assuming UserId is available
                    PaymentId = payment.PaymentId, // Use the newly created PaymentId
                    Status = "Pending", // Set initial status
                    TotalAmount = grandTotal, // Calculate the total amount based on cart
                    OrderDate = DateTime.Now,
                    ShippingAddress = payment.Address
                };
                db.Orders.Add(order);
                db.SaveChanges(); // Save the order to get its OrderId

                // Fetch cart items


                // Process each cart item and create order details
                foreach (var cartItem in cartItems)
                {
                    // Find the product in the Product table
                    var product = db.Products.Find(cartItem.ProductId);

                    if (product != null)
                    {
                        // Reduce product stock
                        product.UnitInStock -= cartItem.Quantity;

                        // change status when product becomes zero
                        if (product.UnitInStock <= 0)
                        {
                            product.StockStatus = 0;
                        }

                        // Update product stock in the database
                        db.Entry(product).State = EntityState.Modified;

                        // Create order details
                        OrderDetails orderDetails = new OrderDetails
                        {
                            OrderId = order.OrderId, // Use the newly created OrderId
                            ProductId = cartItem.ProductId,
                            Quantity = cartItem.Quantity,
                            TotalPrice = (int)(cartItem.Quantity * product.UnitPrice) // Calculate total price for each product
                        };
                        db.OrderDetails.Add(orderDetails);
                    }
                }

                // Save changes for updated product stock and order details
                db.SaveChanges();

                // Clear the cart after successful payment
                db.Carts.RemoveRange(cartItems);
                db.SaveChanges();
                TempData["MsgSuccess"] = "Order has been placed successfully";

                return RedirectToAction("Invoice", "Payments", new { orderId = order.OrderId }); // Redirect to a success page
            }

            return View(payment);
        }































































        public ActionResult Invoice(int orderId)
        {
            int userId = Convert.ToInt32(Session["id"]);

            // Fetch the current order for the logged-in user based on the orderId
            var invoices = (from order in db.Orders
                            join orderDetail in db.OrderDetails on order.OrderId equals orderDetail.OrderId
                            join product in db.Products on orderDetail.ProductId equals product.ProductId
                            where order.UserId == userId && order.OrderId == orderId
                            select new InvoiceViewModel
                            {
                                OrderNo = order.OrderNo,
                                ProductName = product.Name,
                                UnitPrice = (double)product.UnitPrice,
                                Quantity = orderDetail.Quantity,
                                TotalPrice = orderDetail.TotalPrice,
                                TotalAmount = (double)order.TotalAmount
                            }).ToList();

            // Ensure we're only sending the details of the current order
            if (invoices == null || invoices.Count == 0)
            {
                return HttpNotFound("No order found.");
            }

            return View(invoices);
        }

        public ActionResult DownloadInvoice(int orderNo)
        {
            int userId = Convert.ToInt32(Session["id"]);
           
            var orderId = db.Orders
                    .Where(o => o.OrderNo == orderNo) // Filter by orderNo
                    .Select(o => o.OrderId) // Select only the OrderId
                    .FirstOrDefault();
            var invoices = (from order in db.Orders
                            join orderDetail in db.OrderDetails on order.OrderId equals orderDetail.OrderId
                            join product in db.Products on orderDetail.ProductId equals product.ProductId
                            where order.UserId == userId && order.OrderId == orderId
                            select new InvoiceViewModel
                            {
                                OrderNo = order.OrderNo,
                                ProductName = product.Name,
                                UnitPrice = (double)product.UnitPrice,
                                Quantity = orderDetail.Quantity,
                                TotalPrice = orderDetail.TotalPrice,
                                TotalAmount = (double)order.TotalAmount
                            }).ToList();

            // Ensure we're only sending the details of the current order
            if (invoices == null || invoices.Count == 0)
            {
                return HttpNotFound("No order found.");
            }

            return new ViewAsPdf("Invoicepdf", invoices)
            {
                FileName = $"Invoice_{orderNo}.pdf" // Customize the file name
            };
        }





        // GET: Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PaymentId,UserName,CardNo,ExpDate,Cvv,Address,PaymentMode")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(payment);
        }

        // GET: Payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payment payment = db.Payments.Find(id);
            db.Payments.Remove(payment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

