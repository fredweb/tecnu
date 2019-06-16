using System.Web.Mvc;

namespace Tecnu
{
    public class ModuleAreaRegistration : AreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Student_default",
                url: "Cadastros/{controller}/{action}/{id}",
                defaults: new { area = "Tecnu", id = UrlParameter.Optional }
            );
        }

        public override string AreaName
        {
            get
            {
                return "Tecnu";
            }
        }
    }
}