using Microsoft.Owin;
using XNuvem.Logging;

namespace XNuvem.Environment
{
    public class DefaultShellEventHandler : IShellEvents
    {
        public DefaultShellEventHandler()
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void OnBeginRequest(IOwinContext context)
        {
            Logger.Debug("On begin request - " + context.Request.Uri);
        }

        public void OnEndRequest(IOwinContext context)
        {
            Logger.Debug("On end request - " + context.Request.Uri);
        }

        public void OnInitialize()
        {
            Logger.Debug("On application initialize");
        }

        public void OnTerminate()
        {
            Logger.Debug("On application terminate");
        }
    }
}