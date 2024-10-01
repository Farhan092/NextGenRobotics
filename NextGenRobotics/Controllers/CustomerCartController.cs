using NextGenRobotics.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NextGenRobotics.Context;

namespace NextGenRobotics.Controllers
{
    public class CustomerCartController : Controller
    {
        private AspRoboDB db = new AspRoboDB();

        // GET: Carts
        public ActionResult Index()
        {
            if (Session["username"] != null && (Session["Role"].ToString().Equals("Customer") || Session["Role"].ToString().Equals("Admin")))
            {
                int userId = Convert.ToInt32(Session["id"]);
                var cartItems = db.Carts.Where(c => c.UserId == userId).ToList();
            return View(cartItems);

        }
            return RedirectToAction("Login", "Users");
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity)
        {
            if (Session["username"] == null)
            {
              
                TempData["LoginRequiredMessage"] = "You must log in to add items to the cart.";

                
                return RedirectToAction("LandingPageView","Users");
            }


            int userId = Convert.ToInt32(Session["id"]); // Get the current user's ID here
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product == null || product.UnitInStock < quantity)
            {
                // If product doesn't exist or stock is insufficient
                TempData["MsgQn"] = "Insufficient stock to add to cart.";
                return RedirectToAction("LandingPageView", "Users");
            }

            var existingCartItem = db.Carts.FirstOrDefault(c => c.ProductId == productId && c.UserId == userId);

            if (existingCartItem != null)
            {
                // Check if the total quantity exceeds available stock
                if (existingCartItem.Quantity + quantity > product.UnitInStock)
                {
                    TempData["MsgQn"] = "Insufficient stock to add more to the cart.";
                    return RedirectToAction("LandingPageView", "Users");
                }

                // Update quantity if item already exists in the cart
                existingCartItem.Quantity += quantity;
            }
            else
            {
                // Create a new cart item
                var newCartItem = new Cart
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UserId = userId
                };
                db.Carts.Add(newCartItem);
            }

            db.SaveChanges();
            TempData["Msg"] = "Added to cart successfully";
            return RedirectToAction("LandingPageView", "Users");
        }

        [HttpPost]
        public ActionResult UpdateCartQuantity(int productId, int quantity)
        {
            int userId = Convert.ToInt32(Session["id"]); // Get the current user's ID here
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product == null || product.UnitInStock < quantity)
            {
                // If product doesn't exist or stock is insufficient
                TempData["MsgQn"] = "Insufficient stock to update cart.";
                return RedirectToAction("Index");
            }

            var existingCartItem = db.Carts.FirstOrDefault(c => c.ProductId == productId && c.UserId == userId);

            if (existingCartItem != null)
            {
                // Update quantity if the new quantity is within stock limits
                if (quantity > product.UnitInStock)
                {
                    TempData["MsgQn"] = "Insufficient stock to update the cart.";
                    return RedirectToAction("Index");
                }

                existingCartItem.Quantity = quantity;
            }
            else
            {
                // Handle case where item is not found in the cart
                return View();
            }

            db.SaveChanges();
            return RedirectToAction("Index"); // Redirect back to refresh the cart view
        }

        // GET: Carts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: Carts/Create



        // POST: Carts/Delete/5
        public ActionResult Delete(int id)
        {
            var data = db.Carts.Where(x => x.CartId == id).First();
            db.Carts.Remove(data);
            db.SaveChanges();
            TempData["MsgRem"] = "Product successfully removed";

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
