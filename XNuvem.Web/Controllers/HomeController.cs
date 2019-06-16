using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XNuvem.Data;
using XNuvem.Logging;
using XNuvem.Security;
using XNuvem.UI.Navigation;

namespace XNuvem.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRepository<User> _users;
        private readonly IMenuManager _menuManager;
        private readonly IUserService _userService;

        public ILogger Logger { get; set; }
        public HomeController(IUserService userService, IRepository<User> users, IMenuManager menuManager) {
            _userService = userService;
            _users = users;
            _menuManager = menuManager;
        }

        public ActionResult Index() {
            return View();
        }
        
        public ActionResult About() {
            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult Erros() {
            throw new NotImplementedException("Não foi implementado ainda");
        }
    }
}