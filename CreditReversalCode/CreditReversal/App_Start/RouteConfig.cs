using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CreditReversal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            //Account
            routes.MapRoute(
               name: "account-signin",
               url: "account/sign-in",
               defaults: new { controller = "Account", action = "SignIn" }
           );
            routes.MapRoute(
              name: "account-signup",
              url: "account/sign-up",
              defaults: new { controller = "Account", action = "SignUp" }
          );

            //Dashboard
            //   routes.MapRoute(
            //    name: "dashboard-agent",
            //    url: "dashboard/agent",
            //    defaults: new { controller = "Dashboard", action = "Agent" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                 defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                 //defaults: new { controller = "Simple", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
