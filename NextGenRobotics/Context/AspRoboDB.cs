using NextGenRobotics.Models;
using NextGenRobotics.Views.Product;
using System;
using System.Data.Entity;
using System.Linq;

namespace NextGenRobotics.Context
{
    public class AspRoboDB : DbContext
    {
        // Your context has been configured to use a 'AspRoboDB' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'NextGenRobotics.Context.AspRoboDB' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'AspRoboDB' 
        // connection string in the application configuration file.
        public AspRoboDB()
            : base("name=AspRoboDB")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Cart> Carts { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }



        public virtual DbSet<OrderDetails> OrderDetails { get; set; }

    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}