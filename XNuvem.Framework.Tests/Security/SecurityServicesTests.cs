using Autofac;
using Microsoft.AspNet.Identity;
using NHibernate.AspNet.Identity;
using NHibernate.Linq;
using NUnit.Framework;
using System.Linq;
using System.Security.Claims;
using XNuvem.Data;
using XNuvem.Data.Providers;
using XNuvem.Environment.Configuration;
using XNuvem.Logging;
using XNuvem.Security;
using XNuvem.Tests.Data.Stubs;

namespace XNuvem.Tests.Security
{
    [TestFixture]
    class SecurityServicesTests
    {
        private const string UnitOfWork = "UnitOfWork";

        [Test]
        public void CheckUserAndClaims() {
            var container = GetIOC();

            using (var work = container.BeginLifetimeScope(UnitOfWork)) {
                var userService = work.Resolve<IUserService>();
                var user = userService.FindByName("marvin");
                if (user != null) {
                    userService.Delete(user);
                }

                user = new User();
                user.UserName = "marvin";
                user.Email = "marvin@hotmail.com";
                userService.UserManager.Create(user, "password");

                var claim = new Claim("Operation", "Add");
                userService.UserManager.AddClaim(user.Id, claim);

                user = userService.FindByName(user.UserName);
                Assert.That(user.Claims.Count, Is.EqualTo(1));
                Assert.That(user.Claims.FirstOrDefault().ClaimType, Is.EqualTo("Operation"));
            }

        }

        private IContainer GetIOC() {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LoggingModule());
            builder.RegisterType<ShellSettingsManager>().As<IShellSettingsManager>().SingleInstance();
            builder.RegisterType<SessionConfigurationCacheStub>().As<ISessionConfigurationCache>().InstancePerDependency();
            builder.RegisterType<DefaultDataServiceProvider>().As<IDataServiceProvider>().InstancePerDependency();
            builder.RegisterType<SessionFactoryHolder>().As<ISessionFactoryHolder>().SingleInstance();
            builder.RegisterType<TransactionManager>().As<ITransactionManager>().InstancePerMatchingLifetimeScope(UnitOfWork);
            builder.RegisterModule(new DataModule());
            builder.RegisterType<XNuvemUserService>().As<IUserService>().InstancePerDependency();
            builder.RegisterType<XNuvemSignInService>().As<ISignInService>().InstancePerDependency();

            return builder.Build();
        }

        [OneTimeSetUp]
        public void Initialize() {
            //log4net.Config.BasicConfigurator.Configure(new log4net.Appender.ConsoleAppender());
            var hierarchy = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();

            var patternLayout = new log4net.Layout.PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();

            var console = new log4net.Appender.ConsoleAppender();
            console.Layout = patternLayout;

            hierarchy.Root.AddAppender(console);
            hierarchy.Root.Level = log4net.Core.Level.All;
            hierarchy.Configured = true;
        }
    }
}
