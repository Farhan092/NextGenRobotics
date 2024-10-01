using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NextGenRobotics
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



/*            routes.MapRoute(
        name: "ProductDetails",
        url: "Products/Details/{id}",
        defaults: new { controller = "Products", action = "Details", id = UrlParameter.Optional }
    );*/
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Users", action = "LandingPageView", id = UrlParameter.Optional }
            );

        }
    }
}
