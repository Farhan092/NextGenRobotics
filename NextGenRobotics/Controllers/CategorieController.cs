using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NextGenRobotics.Context;
using NextGenRobotics.Models;

namespace NextGenRobotics.Controllers
{
    public class CategorieController : Controller
    {
        private AspRoboDB db = new AspRoboDB();

        // GET: Categorie
        public ActionResult Index()
        {
            if (Session["username"] != null && Session["Role"].ToString().Equals("Admin"))
            {
                return View(db.Categories.ToList());


        }
            return RedirectToAction("Login", "Users");
        }

        // GET: Categorie/Details/5
        /*        public ActionResult Details(int? id)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Category category = db.Categories.Find(id);
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    return View(category);
                }*/

        // GET: Categorie/Details/5
        public ActionResult Details(int? id)
        {


                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Fetch the category along with its related products
            Category category = db.Categories
                                 .Include(c => c.Products)  // Eager loading of related products
                                 .FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }


        // GET: Categorie/Create
        public ActionResult Create()
        {
            if (Session["username"] != null && Session["Role"].ToString().Equals("Admin"))
            {
                return View();
        }
            return RedirectToAction("Login", "Users");
        }

        // POST: Categorie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();

                TempData["add"] = "Category Added Successfully";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categorie/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["username"] != null && Session["Role"].ToString().Equals("Admin"))
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
            return RedirectToAction("Login", "Users");
        }

        // POST: Categorie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                TempData["edit"] = "Category Edited Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categorie/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["username"] != null && Session["Role"].ToString().Equals("Admin"))
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
            return RedirectToAction("Login", "Users");
        }

        // POST: Categorie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();

            TempData["delete"] = "Category Deleted Successfully";
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
