using System.Web.Mvc;

namespace XNuvem.Mvc
{
    public class XNuvemViewEngine : RazorViewEngine
    {
        public XNuvemViewEngine()
        {
            AreaViewLocationFormats = new[]
            {
                "~/Modules/{2}/Views/{1}/{0}.cshtml",
                "~/Modules/{2}/Views/Shared/{0}.cshtml"
            };

            AreaMasterLocationFormats = new[]
            {
                "~/Modules/{2}/Views/{1}/{0}.cshtml",
                "~/Modules/{2}/Views/Shared/{0}.cshtml"
            };

            AreaPartialViewLocationFormats = new[]
            {
                "~/Modules/{2}/Views/{1}/{0}.cshtml",
                "~/Modules/{2}/Views/Shared/{0}.cshtml"
            };

            ViewLocationFormats = new[]
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };

            MasterLocationFormats = new[]
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };

            PartialViewLocationFormats = new[]
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };
        }
    }
}