using NextGenRobotics.Context;
using NextGenRobotics.Models;
using NextGenRobotics.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace NextGenRobotics.Controllers
{
    public class CustomerProductController : Controller
    {

        private AspRoboDB db = new AspRoboDB();
        private HttpClient client;

        public CustomerProductController()
        {
            this.client = new HttpClient();
        }
        // GET: CustomerProduct
        public ActionResult Index()
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

    }
}