using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace XNuvem.Mvc
{
    public static class RouteExtentions
    {
        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults,
            object constraints, string[] namespaces)
        {
            if (routes == null) throw new ArgumentNullException("routes");
            if (url == null) throw new ArgumentNullException("url");

            var route = new LowercaseRoute(url, new MvcRouteHandler())
            {
                Defaults = CreateRouteValueDictionary(defaults),
                Constraints = CreateRouteValueDictionary(constraints),
                DataTokens = new RouteValueDictionary()
            };

            if (namespaces != null && namespaces.Length > 0) route.DataTokens["Namespaces"] = namespaces;


            routes.Add(name, route);

            return route;
        }

        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults,
            object constraints)
        {
            return MapRouteLowerCase(routes, name, url, defaults, constraints, null);
        }

        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults)
        {
            return MapRouteLowerCase(routes, name, url, defaults, null, null);
        }

        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url)
        {
            return MapRouteLowerCase(routes, name, url, null, null, null);
        }

        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, string[] namespaces)
        {
            return MapRouteLowerCase(routes, name, url, null /* defaults */, null /* constraints */, namespaces);
        }

        public static Route MapRouteLowerCase(this RouteCollection routes, string name, string url, object defaults,
            string[] namespaces)
        {
            return MapRouteLowerCase(routes, name, url, defaults, null /* constraints */, namespaces);
        }

        private static RouteValueDictionary CreateRouteValueDictionary(object values)
        {
            var dictionary = values as IDictionary<string, object>;
            if (dictionary != null) return new RouteValueDictionary(dictionary);

            return new RouteValueDictionary(values);
        }
    }
}