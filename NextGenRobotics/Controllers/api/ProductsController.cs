using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using NextGenRobotics.Context;
using NextGenRobotics.Models;

namespace NextGenRobotics.Controllers.api
{
    public class ProductsController : ApiController
    {
        private AspRoboDB db;
        public ProductsController()
        {
            this.db = new AspRoboDB();
        }
        // GET: api/Products
        [Route("api/products")]
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            return db.Products.ToList();
        }

        // GET: api/Products/5
        /*        [Route("api/products/{id}")]
                public IHttpActionResult GetProduct(int id)
                {

                    var product = db.Products.FirstOrDefault(g => g.ProductId == id);

                    if (product == null)
                    {
                        return NotFound();
                    }
                    db.Entry(product).Reference(p => p.Category).Load();

                    return Ok(product);
                }
        */

        // GET: api/Products/5
        [Route("api/products/{id}")]
        public IHttpActionResult GetProduct(int id)
        {
            var product = db.Products
                .Include(p => p.Category) // Include the Category
                .FirstOrDefault(g => g.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var productDetails = new
            {
                product.ProductId,
                product.Name,
                product.UnitPrice,
                product.UnitInStock,
                product.StockStatus,
                product.Description,
                product.PicturePath,
                CategoryName = product.Category != null ? product.Category.Name : "Not Available" // Include category name
            };

            return Ok(productDetails);
        }




        // PUT: api/Products/5
        /*        [Route("api/updateproducts/{id}")]
                [HttpPut]
                public IHttpActionResult UpdateProduct(int id, Product product)
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    if (id != product.ProductID)
                    {
                        return BadRequest();
                    }

                    db.Entry(product).State = EntityState.Modified;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProductExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return StatusCode(HttpStatusCode.NoContent);
                }
        */


        // POST: api/Products
        [Route("api/addproducts")]
        [HttpPost]
        public IHttpActionResult AddProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(product);
            db.SaveChanges();



            return Ok(product);
        }


        // PUT: api/Products/5
        /*        [Route("api/updateproducts/{id}")]
                [HttpPut]
                public IHttpActionResult UpdateProduct(int id, Product product)
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    if (id != product.ProductID)
                    {
                        return BadRequest();
                    }

                    db.Entry(product).State = EntityState.Modified;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProductExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return StatusCode(HttpStatusCode.NoContent);
                }
        */


        [Route("api/updateproducts/{id}")]
        [HttpPut]
        public IHttpActionResult UpdateProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductId)
            {
                return BadRequest();
            }


            var existingProduct = db.Products.Find(id);
            if (existingProduct == null)
            {
                return NotFound();
            }


            existingProduct.Name = product.Name;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.UnitPrice = product.UnitPrice;
            existingProduct.UnitInStock = product.UnitInStock;
            existingProduct.StockStatus = product.StockStatus;
            existingProduct.Description = product.Description;
           

            if (!string.IsNullOrEmpty(product.PicturePath))
            {
                existingProduct.PicturePath = product.PicturePath;
            }

            db.Entry(existingProduct).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }








        // DELETE: api/Products/5
        [Route("api/deleteproducts/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteProduct(int id)
        {
            var product = db.Products.FirstOrDefault(g => g.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductId == id) > 0;
        }
    }
}