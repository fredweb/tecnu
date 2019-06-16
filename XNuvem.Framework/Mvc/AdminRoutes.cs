using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace XNuvem.Mvc
{
    public static class AdminRoutes
    {
        public static void RegisterRoutes(RouteCollection routes, IEnumerable<Assembly> assemblies)
        {
            var controllers = assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => !t.IsAbstract && typeof(Controller).IsAssignableFrom(t) &&
                            t.IsDefined(typeof(AdminAttribute)))
                .ToList();
            foreach (var t in controllers) MapRoute(routes, t);
        }

        internal static RouteBase MapRoute(RouteCollection routes, Type adminType)
        {
            var adminAtt = adminType.GetCustomAttribute<AdminAttribute>();
            var name = string.Concat("settings_", adminAtt.AreaName, adminAtt.GroupName);
            var url = string.Concat("settings/", adminAtt.GroupName, "/{action}/{id}");
            var controllerName = adminType.Name.Substring(0, adminType.Name.IndexOf("Controller"));
            var route = routes.MapRoute(
                name,
                url,
                new {area = adminAtt.AreaName, controller = controllerName, id = UrlParameter.Optional});
            if (!string.IsNullOrEmpty(adminAtt.AreaName)) route.DataTokens.Add("area", adminAtt.AreaName);
            return route;
        }
    }
}