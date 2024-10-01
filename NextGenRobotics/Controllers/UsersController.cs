using NextGenRobotics.Context;

using NextGenRobotics.Models;
using NextGenRobotics.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Project.Controllers
{
    public class UsersController : Controller
    {
        private AspRoboDB db = new AspRoboDB();

        // GET: Users/Index
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,UserName,Password,Email,PhoneNumber,Address,ImageUrl")] User user, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(x => x.UserName == user.UserName))
                {

                    ModelState.Clear();
                    TempData["msgun"] = "Username already exists";
                    return View();
                }
                // Check password confirmation
                if (user.Password != Request.Form["RetypePassword"])
                {
                    ModelState.AddModelError("RetypePassword", "Passwords do not match.");
                    return View(user);
                }

                user.Role = "Customer"; // Hard-code the role

                // Handle image upload
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(ImageFile.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Images"), fileName);
                    ImageFile.SaveAs(path);
                    user.ImageUrl = Url.Content("~/Images/" + fileName);
                }

                else
                {
                    // Set a default image if none is uploaded
                    user.ImageUrl = Url.Content("~/Images/ddddd.jpg"); // Ensure you have a 'default.png' image in the 'uploads' folder
                }

                db.Users.Add(user);
                db.SaveChanges();

                // Pass user information to the login page
                TempData["NewUserName"] = user.UserName;
                TempData["NewUserRole"] = user.Role;
                TempData["SuccessMessage"] = "Account created successfully. Please log in.";

                return RedirectToAction("Login");
            }

            return View(user);
        }






        // GET: Users/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User u)
        {
            // Remove validation for fields not required in login
            ModelState.Remove("RePassword");
            ModelState.Remove("Name");
            ModelState.Remove("Email");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("Address");
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                var data = db.Users.FirstOrDefault(x => x.UserName == u.UserName && x.Password == u.Password);
                if (data != null)
                {
                    Session["id"] = data.UserId;
                    Session["username"] = data.UserName;
                    Session["Role"] = data.Role;
                    Session["Picture"] = data.ImageUrl;

                    // Redirect based on role
                    if (data.Role == "Customer")
                    {
                        return RedirectToAction("LandingPageView", "Users");
                    }
                    else if (data.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Orders");
                    }
                    
                    else
                    {
                        return RedirectToAction("SuperAdminDashboard", "Users");
                    }
                }
                else
                {
                    ViewBag.Invalid = "Invalid username or password";
                    ModelState.Clear();
                    return View();
                }
            }

            ModelState.Clear();
            return View();
        }

        // GET: Users/Clear
        public ActionResult Clear()
        {
            return RedirectToAction("Login");
        }



        /*        public ActionResult Edit(int? id)
                {
                    if (Session["username"] != null && Session["Role"].ToString().Equals("Admin") || Session["Role"].ToString().Equals("SuperAdmin"))
                    {
                        if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    User user = db.Users.Find(id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }

                    // Display welcome message and user picture
                    ViewBag.WelcomeMessage = $"Welcome, {user.Name}!";
                    ViewBag.UserImage = user.ImageUrl;

                    return View(user);
                }
                    return RedirectToAction("Login", "Users");
                }*/





        public ActionResult Edit(int? id)
        {
            // Check if the user is logged in by validating the session
            if (Session["username"] != null)
            {
                // Get the logged-in user's session id
                int sessionId = Convert.ToInt32(Session["id"]);

                // Ensure that the requested id matches the logged-in user's session id
                if (id == null || id != sessionId)
                {
                    // If the id doesn't match or is null, return a "Forbidden" status
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not authorized to edit this profile.");
                }

                // Find the user with the specified id (which should match the session id)
                User user = db.Users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Display welcome message and user picture
                ViewBag.WelcomeMessage = $"Welcome, {user.Name}!";
                ViewBag.UserImage = user.ImageUrl;

                return View(user);
            }

            // If the user is not logged in, redirect to the login page
            return RedirectToAction("Login", "Users");
        }


        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Name,UserName,Password,Email,PhoneNumber,Address,ImageUrl")] User user, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the existing user from the database
                var existingUser = db.Users.Find(user.UserId);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                // Update the user properties
                existingUser.Name = user.Name;
                existingUser.UserName = user.UserName;
                existingUser.Password = user.Password; // You might want to hash this before saving
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;

                // Keep the role as "Customer"

               // existingUser.Role = "Customer";
                

                // Handle image upload if there's a new image
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(ImageFile.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Images"), fileName);
                    ImageFile.SaveAs(path);
                    existingUser.ImageUrl = Url.Content("~/Images/" + fileName);
                }

                // Save changes to the database
                db.SaveChanges();
                return RedirectToAction("Details", new { id = Session["id"] });

            }

            ViewBag.UserImage = user.ImageUrl; // Show the existing image if model state is invalid
            ViewBag.WelcomeMessage = $"Welcome, {user.Name}!"; // Keep welcome message
            return View(user);
        }

        /*        public ActionResult Details(int? id)
                {

                    if ((Session["username"] != null && Session["Role"].ToString().Equals("Admin")) || Session["Role"].ToString().Equals("SuperAdmin"))
                    {


                        // Check if the id is null
                        if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    // Find the user with the specified id
                    User user = db.Users.Find(id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }

                    // Pass the user object to the view
                    return View(user);
                }
                    return RedirectToAction("Login", "Users");
                }*/

        public ActionResult Details(int? id)
        {
            // Check if the user is logged in by validating the session
            if (Session["username"] != null)
            {
                // Get the logged-in user's session id
                int sessionId = Convert.ToInt32(Session["id"]);

                // Ensure that the requested id matches the logged-in user's session id
                if (id == null || id != sessionId)
                {
                    // If the id doesn't match or is null, return a "Forbidden" status
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not authorized to view this profile.");
                }

                // Find the user with the specified id (which should match the session id)
                User user = db.Users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Pass the user object to the view
                return View(user);
            }

            // If the user is not logged in, redirect to the login page
            return RedirectToAction("Login", "Users");
        }



        public ActionResult ViewCustomer()
        {
            if (Session["username"] != null && Session["Role"].ToString().Equals("Admin"))
            {
                var admins = db.Users.Where(u => u.Role == "Customer").ToList();

                return View(admins);
            }
            return RedirectToAction("Login", "Users");

            //return View(db.Products.ToList());
        }








        /////////////////superadmin//////////////////


        public ActionResult SuperAdminDashboard()
        {
            if (Session["username"] != null && Session["Role"].ToString().Equals("SuperAdmin"))
            {
                var admins = db.Users.Where(u => u.Role == "Admin").ToList();

            return View(admins);
        }
            return RedirectToAction("Login", "Users");

            //return View(db.Products.ToList());
        }


        public ActionResult AddAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAdmin(User u, HttpPostedFileBase ImageFile)
        {
            if (db.Users.Any(x => x.UserName == u.UserName))
            {

                ModelState.Clear();
                TempData["msg1"] = "Username already exists";
                return View();
            }
            u.Role = "Admin"; // Hard-code the role

            // Handle image upload
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                var fileName = System.IO.Path.GetFileName(ImageFile.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/Images"), fileName);
                ImageFile.SaveAs(path);
                u.ImageUrl = Url.Content("~/Images/" + fileName);
            }

            else
            {
                // Set a default image if none is uploaded
                u.ImageUrl = Url.Content("~/Images/ddddd.jpg"); // Ensure you have a 'default.png' image in the 'uploads' folder
            }
            db.Users.Add(u);
            db.SaveChanges();
            return View("Login");
        }


        public ActionResult DeleteAdmin(int id)
        {
            var data = db.Users.Where(x => x.UserId == id).First();
            db.Users.Remove(data);
            db.SaveChanges();
            //TempData["MsgRem"] = "Employee information removed successfully";

            return RedirectToAction("SuperAdminDashboard");
        }

        /*        public ActionResult LandingPageView()
                {
                    //var users = db.Products.ToList();



                    var categories = db.Categories.ToList(); // Assuming your DbSet for categories is 'Categories'

                    // Check if categories are being retrieved correctly
                    if (categories == null || !categories.Any())
                    {
                        // Handle the case where no categories are found, or the result is null
                        ViewBag.Categories = new List<Category>(); // Set an empty list to avoid null reference
                    }
                    else
                    {
                        // Pass categories to the view
                        ViewBag.Categories = categories;
                    }

                    return View();

                }*/


        public ActionResult LandingPageView()
        {
            // Ensure categories are retrieved from the database
            var categories = db.Categories.ToList();

            // Check if categories are not null before assigning to ViewBag
            if (categories != null)
            {
                ViewBag.Categories = categories;
            }

            // Continue with the rest of the ViewModel
            var products = db.Products.ToList();

            var viewModel = new ProductCategoryVM
            {
                Categories = categories,  // You may also use this directly in the ViewModel instead of ViewBag
                Products = products
            };

            return View(viewModel);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            
            TempData["Logout"] = "Logged out from the system";

            return View("Login");
        }

        public ViewResult ClearLogin()
        {
            ModelState.Clear();
            return View("Login");
        }

        public ViewResult ClearEdit()
        {
            ModelState.Clear();
            return View("Edit");
        }


        public ViewResult ClearSignUp()
        {
            ModelState.Clear();
            return View("Create");
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
