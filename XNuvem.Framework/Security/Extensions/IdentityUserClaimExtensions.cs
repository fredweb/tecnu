using System.Security.Claims;
using NHibernate.AspNet.Identity;

namespace XNuvem.Security
{
    public static class IdentityUserClaimExtensions
    {
        public static Claim GetClaim(this IdentityUserClaim theThis)
        {
            return new Claim(theThis.ClaimType, theThis.ClaimValue);
        }
    }
}