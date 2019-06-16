using System.Collections.Generic;

namespace XNuvem.Security.Permissions
{
    public interface IPermissionProvider
    {
        IEnumerable<Permission> GetPermissions();
    }
}