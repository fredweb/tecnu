using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XNuvem.Security;
using XNuvem.UI.Navigation;
using XNuvem.Web.Models;

namespace XNuvem.Web.Controllers
{
    public class NavigationController : Controller
    {
        private readonly IMenuManager _menuManager;
        private readonly IUserService _userService;

        public NavigationController(IMenuManager menuManager, IUserService userService) {
            _menuManager = menuManager;
            _userService = userService;
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            var user = _userService.GetCurrentUser();
            var model = new NavigationViewModel(_menuManager.GetRootMenu());
            model.CurrentUser = user;
            return View(model);
        }
    }
}