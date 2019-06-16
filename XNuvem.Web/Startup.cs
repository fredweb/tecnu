/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016
 * 
 * 
/****************************************************************************************/

using Autofac;
using FluentValidation.Mvc;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using XNuvem.Mvc;
using XNuvem.Owin;

[assembly: OwinStartupAttribute(typeof(XNuvem.Web.Startup))]
namespace XNuvem.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ModelBinders.Binders.Add(typeof(double), new DoubleModelBinder());
            app.UseXNuvemFramework(Registrations);
        }

        public static void Registrations(ContainerBuilder builder)
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FluentValidationModelValidatorProvider.Configure();
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
        }
    }
}
