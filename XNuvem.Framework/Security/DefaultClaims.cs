using System.Security.Claims;

namespace XNuvem.Security
{
    public static class DefaultClaims
    {
        public static Claim Administrator = new Claim("Role", "Administrator");

        public static Claim SiteUser = new Claim("Role", "SiteUser");
    }
}