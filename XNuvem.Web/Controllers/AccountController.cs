using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using XNuvem.Web.Models;
using XNuvem.Security;
using NHibernate.AspNet.Identity;
using Microsoft.Owin;
using XNuvem.UI.Messages;

namespace XNuvem.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    { 
        private readonly ISignInService _signInManager;
        private readonly IUserService _userManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IDisplayMessages _displayMessages;

        public AccountController(
            IUserService userManager, 
            ISignInService signInManager,
            IAuthenticationManager authentication,
            IDisplayMessages displayMessages)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationManager = authentication;
            _displayMessages = displayMessages;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl) {
            if (!ModelState.IsValid) {
                return View(model);
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result) {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    _displayMessages.Error("Usuário ou senha incorreto. Verifique se as informações estão corretas.");
                    ViewBag.ErroMessage = "Usuário ou senha incorreto. Verifique se as informações estão corretas.";
                    return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logoff() {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public ActionResult SignOut() {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        #region Helpers...
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}