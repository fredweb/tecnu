using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XNuvem.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult NotFound() {
            Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = 404;
            return View();
        }

        [HttpGet]
        public ActionResult ServerError() {
            Response.StatusCode = 500;
            return View();
        }
    }
}