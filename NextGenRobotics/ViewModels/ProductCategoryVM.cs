using NextGenRobotics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextGenRobotics.ViewModels
{
    public class ProductCategoryVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}