/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 *
 * 
/****************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Autofac;
using Autofac.Integration.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using NHibernate;
using NHibernate.AspNet.Identity;
using XNuvem.Data;
using XNuvem.Logging;
using XNuvem.Security.Permissions;

namespace XNuvem.Security
{
    public class XNuvemUserService : UserManager<User>, IUserService
    {
        private readonly ITransactionManager _transactionManager;

        public XNuvemUserService(ITransactionManager transactionManager)
            : base(new UserStore<User>(transactionManager.GetSession())
            {
                ShouldDisposeSession = false
            })
        {
            _transactionManager = transactionManager;
            SetAccountConfiguration();
            Logger = NullLogger.Instance;
        }

        public ISession Session => _transactionManager.GetSession();

        public ILogger Logger { get; set; }

        public override Task<ClaimsIdentity> CreateIdentityAsync(User user, string authenticationType)
        {
            Logger.Debug("Creating identity for user {0}", user.UserName);
            return base.CreateIdentityAsync(user, authenticationType);
        }

        public static UserManager<User> Create(IdentityFactoryOptions<UserManager<User>> options, IOwinContext context)
        {
            return context
                .GetAutofacLifetimeScope()
                .Resolve<IUserService>()
                .UserManager;
        }


        internal void SetAccountConfiguration()
        {
            // Configure logic for user names
            UserValidator = new UserValidator<User>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // Configure validation logic for password
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            // configure user lockout defaults
            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;
        }

        #region IUserService...

        UserManager<User> IUserService.UserManager => this;

        void IUserService.Create(User user, string password)
        {
            this.Create(user, password);
        }

        void IUserService.Update(User user)
        {
            this.Update(user);
        }

        void IUserService.Delete(User user)
        {
            this.Delete(user);
        }

        User IUserService.FindByName(string userName)
        {
            return this.FindByName(userName);
        }

        IQueryable<User> IUserService.Users => Users;

        User IUserService.GetCurrentUser()
        {
            if (!HostingEnvironment.IsHosted || HttpContext.Current == null)
            {
                Logger.Debug("Não é possível acessar o contexto da execução web.");
                return null;
            }
            var identity = HttpContext.Current.User;
            if (identity == null || identity.Identity == null || string.IsNullOrEmpty(identity.Identity.Name))
                return null;

            return this.FindByName(identity.Identity.Name);
        }

        void IUserService.AssignPermissions(User user, IEnumerable<Permission> permissions)
        {
            if (user.Claims != null)
            {
                var claimsToRemove = user.Claims.Where(c => c.ClaimType == "Permission").ToList();
                foreach (var claim in claimsToRemove) this.RemoveClaim(user.Id, claim.GetClaim());
            }
            foreach (var perm in permissions) this.AddClaim(user.Id, new Claim("Permission", perm.Name));
        }

        async Task IUserService.ForceChangePasswordAsync(User user, string newPassword)
        {
            using (var userStore = new UserStore<User>(Session))
            {
                userStore.ShouldDisposeSession = false;
                var passwordHash = PasswordHasher.HashPassword(newPassword);
                await userStore.SetPasswordHashAsync(user, passwordHash);
            }
        }

        #endregion
    }
}