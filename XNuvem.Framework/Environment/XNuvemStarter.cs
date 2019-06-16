/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016
 * 
 * 
/****************************************************************************************/

using System;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using XNuvem.Data;
using XNuvem.Data.Providers;
using XNuvem.Data.Schema;
using XNuvem.Environment.Configuration;
using XNuvem.Exceptions;
using XNuvem.FileSystems.AppData;
using XNuvem.Logging;
using XNuvem.Mvc;
using XNuvem.Owin;
using XNuvem.Security;
using XNuvem.UI;
using XNuvem.UI.Messages;
using XNuvem.UI.Navigation;

namespace XNuvem.Environment
{
    public static class XNuvemStarter
    {
        public static IContainer Start(Action<ContainerBuilder> registrations)
        {
            var builder = new ContainerBuilder();

            // see note: http://docs.autofac.org/en/latest/faq/iis-restart.html
            // to force IIS load assemblies on application domain
            var assemblies = BuildManager.GetReferencedAssemblies()
                .OfType<Assembly>()
                .Where(x => !x.GlobalAssemblyCache)
                .ToArray();


            // Modules registrations
            builder.RegisterModule<OwinModule>();
            builder.RegisterModule<LoggingModule>();

            // System registrations
            builder.RegisterType<XNuvemServices>().As<IServiceContext>().InstancePerDependency();
            builder.RegisterType<AppDataFolderRoot>().As<IAppDataFolderRoot>().InstancePerDependency();
            builder.RegisterType<AppDataFolder>().As<IAppDataFolder>().InstancePerDependency();

            // Shell Registrations
            {
                builder.RegisterType<ShellSettingsManager>().As<IShellSettingsManager>().SingleInstance();

                builder.RegisterAssemblyTypes(assemblies)
                    .Where(t => !t.IsAbstract && typeof(IHostEvents).IsAssignableFrom(t))
                    .As<IHostEvents>()
                    .InstancePerDependency();

                builder.RegisterAssemblyTypes(assemblies)
                    .Where(t => !t.IsAbstract && typeof(IShellEvents).IsAssignableFrom(t))
                    .As<IShellEvents>()
                    .InstancePerDependency();
            }

            // Data registrations
            builder.RegisterType<SessionConfigurationCache>().As<ISessionConfigurationCache>().InstancePerRequest();
            builder.RegisterType<DefaultDataServiceProvider>().As<IDataServiceProvider>().InstancePerDependency();
            builder.RegisterType<DefaultSchemaUpdate>().As<ISchemaUpdate>().InstancePerDependency();
            builder.RegisterType<SessionFactoryHolder>().As<ISessionFactoryHolder>().InstancePerLifetimeScope();
            builder.RegisterType<TransactionManager>().As<ITransactionManager>().InstancePerRequest();

            // Using autoregistration instead
            builder.RegisterModule<DataModule>();

            // Security registrations
            builder.RegisterType<XNuvemUserService>().As<IUserService>().InstancePerDependency();
            builder.RegisterType<XNuvemSignInService>().As<ISignInService>().InstancePerDependency();

            // General registrations
            builder.RegisterType<DefaultPolicyException>().As<IPolicyException>().InstancePerDependency();
            builder.RegisterType<DisplayMessages>().As<IDisplayMessages>().InstancePerRequest();

            // UI Registrations
            // Using auto registration instead
            builder.RegisterModule<UIModule>();

            // Not in module to avoid reload assemblies
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => !t.IsAbstract && typeof(IMenuProvider).IsAssignableFrom(t))
                .As<IMenuProvider>()
                .InstancePerDependency();

            // Autofac Mvc registrations            
            builder.RegisterControllers(assemblies);

            builder.RegisterModule<AutofacWebTypesModule>();
            builder.RegisterSource(new ViewRegistrationSource());
            builder.RegisterFilterProvider();

            // Register all modules
            builder.RegisterAssemblyModules(assemblies);

            AdminRoutes.RegisterRoutes(RouteTable.Routes, assemblies);

            registrations(builder);

            return builder.Build();
        }
    }
}