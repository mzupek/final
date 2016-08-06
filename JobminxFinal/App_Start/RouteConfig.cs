using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace JobminxFinal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
             name: "Indeed",
             url: "Indeed/GetJobFromIndeed",
             defaults: new { controller = "Indeed", action = "GetJobFromIndeed" });

            routes.MapRoute(
               name: "IndeedJob",
               url: "Indeed/SearchIndeed",
               defaults: new { controller = "Indeed", action = "SearchIndeed" });

            routes.MapRoute(
               name: "LogOff",
               url: "Account/LogOff",
               defaults: new { controller = "Account", action = "LogOff" });


            routes.MapRoute(
                name: "Resume",
                url: "App/ResumeDocx",
                defaults: new { controller = "App", action = "ResumeDocx" });

            routes.MapRoute(
                name: "Login",
                url: "Account/Login",
                defaults: new { controller = "Account", action = "Login" });

            routes.MapRoute(
                name: "Register",
                url: "Account/Register",
                defaults: new { controller = "Account", action = "Register" });

            routes.MapRoute(
                name: "Default",
                url: "{*url}",
                defaults: new { controller = "Home", action = "Index" });
            //DownloadApply

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            
        }
    }
}
