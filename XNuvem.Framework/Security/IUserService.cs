/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 *
 * 
/****************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using XNuvem.Security.Permissions;

namespace XNuvem.Security
{
    public interface IUserService
    {
        UserManager<User> UserManager { get; }
        IQueryable<User> Users { get; }
        void Create(User user, string password);
        void Update(User user);
        void Delete(User user);
        User FindByName(string userName);
        User GetCurrentUser();
        void AssignPermissions(User user, IEnumerable<Permission> permissions);
        Task ForceChangePasswordAsync(User user, string newPassword);
    }
}