using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace XNuvem.Security
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string _claimType;
        private readonly string _claimValue;

        public ClaimsAuthorizeAttribute(string type, string value)
        {
            _claimType = type;
            _claimValue = value;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = HttpContext.Current.User as ClaimsPrincipal;
            if (user.HasClaim(_claimType, _claimValue)) base.OnAuthorization(filterContext);
            else filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}