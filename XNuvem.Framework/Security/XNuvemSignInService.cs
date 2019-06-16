using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using XNuvem.Logging;

namespace XNuvem.Security
{
    public class XNuvemSignInService : SignInManager<User, string>, ISignInService
    {
        private readonly IOwinContext _owinContext;
        private readonly IUserService _userService;

        public XNuvemSignInService(IUserService userService, IOwinContext owinContext) :
            base(userService.UserManager, owinContext.Authentication)
        {
            _userService = userService;
            _owinContext = owinContext;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return _userService.UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        }

        #region ISignInService...

        SignInManager<User, string> ISignInService.SignInManager => this;

        async Task<SignInStatus> ISignInService.PasswordSignInAsync(string userName, string password, bool isPersistent,
            bool shouldLockout)
        {
            var result = await PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
            if (result != SignInStatus.Success)
                Logger.Warning("Falha ao tentar efetuar o login. Usuário: {0}", userName);
            return result;
        }

        void ISignInService.SignOut()
        {
            _owinContext.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        #endregion
    }
}