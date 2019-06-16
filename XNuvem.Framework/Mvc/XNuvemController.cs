using System.Web.Mvc;

namespace XNuvem.Mvc
{
    public abstract class XNuvemController : Controller
    {
        protected ActionResult ViewOrAjax(object model, JsonRequestBehavior jsonBehavior)
        {
            if (Request.IsAjaxRequest()) return Json(model, jsonBehavior);
            return View(model);
        }

        protected ActionResult ViewOrAjax(object model)
        {
            return ViewOrAjax(model, JsonRequestBehavior.DenyGet);
        }

        protected ActionResult AjaxError(string message)
        {
            var errorModel = new
            {
                IsError = true,
                Messages = new[] {message}
            };
            return Json(errorModel);
        }
    }
}