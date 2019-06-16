using System.Web;
using Autofac;
using Autofac.Integration.Owin;
using Microsoft.Owin;

namespace XNuvem.Environment
{
    public class XNuvemServices : IServiceContext
    {
        private readonly IOwinContext _owinContext;

        public XNuvemServices(IOwinContext context)
        {
            _owinContext = context;
        }

        public static IServiceContext Current => new XNuvemServices(HttpContext.Current.GetOwinContext());

        private ILifetimeScope Services => _owinContext.GetAutofacLifetimeScope();

        public TService Resolve<TService>()
        {
            return Services.Resolve<TService>();
        }

        public bool TryResolve<TService>(out TService service)
        {
            service = default(TService);
            try
            {
                service = Services.Resolve<TService>();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}