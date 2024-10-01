using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NextGenRobotics.Context;
using NextGenRobotics.Models;
using NextGenRobotics.ViewModels;

namespace NextGenRobotics.Controllers
{
    public class ProductController : Controller
    {
        private AspRoboDB db = new AspRoboDB();
        private HttpClient client;

        public ProductController()
        {
            this.client = new HttpClient();
        }


        public ActionResult Index()
        {
            if(Session["username"] != null && Session["Role"].ToString().Equals("Admin"))
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(@"http://localhost:50244/api/products");
                    var response = client.GetAsync("products");
                    response.Wait();

                    if (response.Result.IsSuccessStatusCode)
                    {
                        var data = response.Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
                        return View(data);
                    }
                    else
                        return HttpNotFound();
                }
            }
            return RedirectToAction("Login", "Users");

            //return View(db.Products.ToList());
        }


        public ActionResult Details(int? id)
        {


            client.BaseAddress = new Uri(@"http://localhost:50244/api/products");
            var response = client.GetAsync("products/" + id.ToString()).Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsAsync<dynamic>().Result; // Get dynamic data

                // Create and populate the view model
                var viewModel = new ProductDetailsVM
                {
                    ProductId = data.ProductId,
                    Name = data.Name,
                    UnitPrice = data.UnitPrice,
                    UnitInStock = data.UnitInStock != null ? (int)data.UnitInStock : 0,
                    StockStatus = data.StockStatus,
                    Description = data.Description,
                    PicturePath = data.PicturePath,
                    CategoryName = data.CategoryName // Get category name from the response
                };

                return View(viewModel); // Pass the view model to the view
            }
            return HttpNotFound();
        }





        // GET: Product/Create
        public ActionResult Create()
        {
            if (Session["username"] != null && Session["Role"].ToString().Equals("Admin"))
            {
            var categories = db.Categories.Select(c => new { c.CategoryId, c.Name }).ToList();
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "Name");
            return View();
        }
            return RedirectToAction("Login", "Users");
        }


        // POST: Product/Create
        /*
                [HttpPost]
                [ValidateAntiForgeryToken]
                public ActionResult Create(Product product, HttpPostedFileBase file)
                {

                    if (ModelState.IsValid)
                    {

                        if (file != null && file.ContentLength > 0)
                        {
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                            var extension = Path.GetExtension(file.FileName).ToLower();

                            if (!allowedExtensions.Contains(extension))
                            {
                                ModelState.AddModelError("file", "Please upload a valid image file (jpg, jpeg, png, gif).");
                            }
                            else if (file.ContentLength > 5 * 1024 * 1024)
                            {
                                ModelState.AddModelError("file", "The file size should be less than 5MB.");
                            }
                            else
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                                file.SaveAs(path);
                                product.PicturePath = "~/Images/" + fileName;
                            }
                        }


                        product.CreatedDate = DateTime.Now;



                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri("http://localhost:50244/api/addproducts");


                            var response = client.PostAsJsonAsync("addproducts", product).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Server error. Please contact the administrator.");
                            }
                        }
                    }
                    var categories = db.Categories.Select(c => new { c.CategoryId, c.Name }).ToList();
                    ViewBag.CategoryId = new SelectList(categories, "CategoryId", "Name");
                    return View(product);
                }
        */


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("file", "Please upload a valid image file (jpg, jpeg, png, gif).");
                    }
                    else if (file.ContentLength > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("file", "The file size should be less than 5MB.");
                    }
                    else
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        file.SaveAs(path);
                        product.PicturePath = "~/Images/" + fileName;
                    }
                }

                product.CreatedDate = DateTime.Now;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:50244/api/addproducts");
                    var response = client.PostAsJsonAsync("addproducts", product).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // Set the success message in TempData
                        TempData["SuccessMessage"] = "Product added successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error. Please contact the administrator.");
                    }
                }
            }

            var categories = db.Categories.Select(c => new { c.CategoryId, c.Name }).ToList();
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "Name");
            return View(product);
        }










        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["username"] != null && Session["Role"].ToString().Equals("Admin"))
            {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", product.CategoryId);
            return View(product);
        }

            return RedirectToAction("Login", "Users");
        }




        // POST: Product/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(file.FileName).ToLower();


                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("file", "Please upload a valid image file (jpg, jpeg, png, gif).");
                    }

                    else if (file.ContentLength > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("file", "The file size should be less than 5MB.");
                    }
                    else
                    {

                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        file.SaveAs(path);


                        product.PicturePath = "~/Images/" + fileName;
                    }
                }
                else
                {

                    var existingProduct = db.Products.Find(product.ProductId);
                    if (existingProduct != null)
                    {
                        product.PicturePath = existingProduct.PicturePath;
                    }
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:50244/api/updateproducts");

                    var response = client.PutAsJsonAsync($"updateproducts/{product.ProductId}", product).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["msgedit"] = "Product edited successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {

                        ModelState.AddModelError(string.Empty, "Server error: ");
                    }
                }
            }


            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", product.CategoryId);
            return View(product);
        }



        // GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }







        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            client.BaseAddress = new Uri(@"http://localhost:50244/api/deleteproducts");
            var response = client.DeleteAsync("deleteproducts/" + id.ToString());
            response.Wait();

            if (response.Result.IsSuccessStatusCode)
            {
                //TempData["MsgRem"] = "Product successfully removed";

                return RedirectToAction("Index");
            }
            else
                return HttpNotFound();


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
