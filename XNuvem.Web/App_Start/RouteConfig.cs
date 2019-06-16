using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using XNuvem.Mvc;

namespace XNuvem.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "SettingsInstall",
                url: "settings/install/{action}",
                defaults: new { area = "", controller = "SettingsInstall" }
                );

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "LoginRoute",
                url: "Login",
                defaults: new { area = "", controller = "Account", action = "Login" }
                );

            routes.MapRoute(
                name: "LogoffRoute",
                url: "Logoff",
                defaults: new { area = "", controller = "Account", action = "Logoff" }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "XNuvem.Web.Controllers" }
            );

            routes.LowercaseUrls = true;
        }
    }
}
