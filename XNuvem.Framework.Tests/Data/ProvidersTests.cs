using Autofac;
using FluentNHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNuvem.Data;
using XNuvem.Data.Providers;
using XNuvem.Environment.Configuration;
using XNuvem.Logging;
using XNuvem.Tests.Data.Model;
using XNuvem.Tests.Data.Stubs;

namespace XNuvem.Tests.Data
{
    [TestFixture]
    class ProvidersTests
    {
        private const string UnitOfWork = "UnitOfWork";
        [Test]
        public void DropAndCreateDatabase() {
            var container = GetIOC();

            var shellSettingsManager = container.Resolve<IShellSettingsManager>();
            var dataService = container.Resolve<IDataServiceProvider>();

            var parameters = GetSessionFactoryParameters(shellSettingsManager);
            var cfg = dataService.BuildConfiguration(parameters);
            Assert.That(cfg, Is.Not.Null);
            var se = new SchemaExport(cfg);
            se.Create(true, true);
        }

        [Test]
        public void UpdateDatabaseSchema() {
            var container = GetIOC();
            var loggerFactory = container.Resolve<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(ProvidersTests));


            var shellSettingsManager = container.Resolve<IShellSettingsManager>();
            var dataService = container.Resolve<IDataServiceProvider>();

            var parameters = GetSessionFactoryParameters(shellSettingsManager);
            var cfg = dataService.BuildConfiguration(parameters);
            Assert.That(cfg, Is.Not.Null);
            logger.Debug("-- START DROP AND CREATE DATABASE --");
            var se = new SchemaExport(cfg);
            se.Create(true, true);
            logger.Debug("-- END DROP AND CREATE DATABASE --");

            var newCfg = Fluently.Configure(cfg).Mappings(m => m.FluentMappings.Add(typeof(FoolMap))).BuildConfiguration();
            var su = new SchemaUpdate(newCfg);
            logger.Debug("-- START UPDATE DATABASE --");
            su.Execute(true, true);
            logger.Debug("-- END UPDATE DATABASE --");

            using (var session = newCfg.BuildSessionFactory().OpenSession()) {
                var newFool = new Fool();
                newFool.Name = "Caio Moreno";
                session.Save(newFool);

                var s = session.Query<Fool>().Cacheable().Where(f => f.Name == "Caio Moreno").SingleOrDefault();

                Assert.That(s, Is.Not.Null);
                Assert.That(s.Name, Is.EqualTo("Caio Moreno"));
                Assert.That(s.Id, Is.GreaterThan(0));
            }

            logger.Debug("-- START DROP DATABASE --");
            //(new SchemaExport(newCfg)).Drop(true, true);
            logger.Debug("-- END DROP DATABASE --");
        }

        [Test]
        public void SaveAndCommitDatabase() {
            var container = GetIOC();

            // Transaction 1
            using (var work = container.BeginLifetimeScope(UnitOfWork)) {
                var users = work.Resolve<IRepository<XNuvem.Security.User>>();
                
                var user = new XNuvem.Security.User();
                user.UserName = "Manager";
                user.Email = "manager@xnuvem.com.br";
                user.EmailConfirmed = true;
                users.Create(user);

                var fool = work.Resolve<IRepository<Fool>>();
                fool.Create(new Fool { 
                    Id = 200,
                    Name = "Lucas"
                });
            }

            // Transaction 2
            using (var work = container.BeginLifetimeScope(UnitOfWork)) {
                var users = work.Resolve<IRepository<XNuvem.Security.User>>();

                var user = users.Table.Where(u => u.UserName == "Manager").SingleOrDefault();
                Assert.That(user, Is.Not.Null);
                Assert.That(user.Email, Is.EqualTo("manager@xnuvem.com.br"));
                users.Delete(user);
            }
        }

        #region Helper functions...

        private SessionFactoryParameters GetSessionFactoryParameters(IShellSettingsManager shellSettingsManager) {
            var settings = shellSettingsManager.GetSettings();

            return new SessionFactoryParameters {
                Configurers = new List<ISessionConfigurationEvents>(),
                Provider = settings.ConnectionSettings.DataProvider,
                DataFolder = "Site",
                ConnectionString = settings.ConnectionSettings.DataConnectionString,
                Entities = settings.Entities,
                EntityMaps = settings.EntityMaps
            };
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

            return builder.Build();
        }

        #endregion

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
