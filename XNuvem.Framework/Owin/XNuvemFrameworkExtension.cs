/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016
 * 
 * 
/****************************************************************************************/

using System;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using XNuvem.Environment;
using XNuvem.Exceptions.Filters;
using XNuvem.Mvc;
using XNuvem.Security;

namespace XNuvem.Owin
{
    public static class XNuvemFrameworkExtension
    {
        public static IAppBuilder UseXNuvemFramework(this IAppBuilder app, Action<ContainerBuilder> registrations)
        {
            // Middleware to lower all Url. Must be first
            app.Use<LoweredUrlMiddleware>();

            var container = XNuvemStarter.Start(registrations);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new XNuvemViewEngine());
            GlobalFilters.Filters.Add(new UnhandledExceptionFilter());
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // Autofac MVC Initialization
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();

            // This middleware verify if database is created
            app.Use<DatabaseVerificationMiddleware>();

            // After all registrations
            app.Use<XNuvemMiddleware>();

            // Register the user manager to be used by Identity
            app.CreatePerOwinContext<UserManager<User>>(XNuvemUserService.Create);

            // Security configurations
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(30));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager<User>, User>(
                        TimeSpan.FromMinutes(15),
                        (manager, user) =>
                            user.CreateIdentityAsync(manager, DefaultAuthenticationTypes.ApplicationCookie))
                }
            });

            XNuvemHost.Start(container, app);

            return app;
        }
    }
}