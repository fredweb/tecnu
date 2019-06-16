using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using XNuvem.Environment;
using XNuvem.Logging;
using XNuvem.Utility.Extensions;

namespace XNuvem.Owin
{
    public class XNuvemMiddleware : OwinMiddleware
    {
        public XNuvemMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            var services = new XNuvemServices(context);
            var loggerFactory = services.Resolve<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(IShellEvents));
            var shellEvents = services.Resolve<IEnumerable<IShellEvents>>();

            // Pipes on begin and end request
            shellEvents.Invoke(s => s.OnBeginRequest(context), logger);
            await Next.Invoke(context);
            shellEvents.Invoke(s => s.OnEndRequest(context), logger);
        }
    }
}