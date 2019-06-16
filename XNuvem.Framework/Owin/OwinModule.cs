/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016
 * 
 * 
/****************************************************************************************/

using System.Collections.Generic;
using System.Web;
using Autofac;
using Autofac.Core;
using Microsoft.Owin;

namespace XNuvem.Owin
{
    public class OwinModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            moduleBuilder.Register(LocateOwinContext).As<IOwinContext>().InstancePerDependency();
            moduleBuilder.Register(c => c.Resolve<IOwinContext>().Authentication).InstancePerDependency();
        }

        private static IOwinContext LocateOwinContext(IComponentContext context, IEnumerable<Parameter> parameters)
        {
            // FIX: Mudar a maneira que se localiza o IOwinContext pois isso não é compatível com MVC5 ??
            return HttpContext.Current.GetOwinContext();
        }
    }
}