using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace XNuvem.Security
{
    public interface ISignInService
    {
        SignInManager<User, string> SignInManager { get; }
        Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout);
        void SignOut();
    }
}