using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using Microsoft.Owin.BuilderProperties;
using Owin;
using XNuvem.Logging;
using XNuvem.Utility.Extensions;

namespace XNuvem.Environment
{
    public class XNuvemHost : IXNuvemHost, IServiceContext
    {
        public static XNuvemHost _currentHost;

        private readonly IContainer _container;

        private XNuvemHost(IContainer container)
        {
            Check.IsNotNull(container, "container");
            _container = container;
            Logger = _container.Resolve<ILoggerFactory>().CreateLogger(typeof(XNuvemHost));
        }

        private ILogger Logger { get; }

        public static IXNuvemHost Current => _currentHost;

        TService IServiceContext.Resolve<TService>()
        {
            return _container.Resolve<TService>();
        }

        bool IServiceContext.TryResolve<TService>(out TService service)
        {
            try
            {
                service = _container.Resolve<TService>();
                return true;
            }
            catch (Exception)
            {
                service = default(TService);
                return false;
            }
        }

        public void OnStart()
        {
            var hostEvents = _container.Resolve<IEnumerable<IHostEvents>>();
            var shellEvents = _container.Resolve<IEnumerable<IShellEvents>>();
            hostEvents.Invoke(e => e.OnInitialize(), Logger);
            shellEvents.Invoke(e => e.OnInitialize(), Logger);
        }

        public void OnTerminate()
        {
            var hostEvents = _container.Resolve<IEnumerable<IHostEvents>>();
            var shellEvents = _container.Resolve<IEnumerable<IShellEvents>>();
            hostEvents.Invoke(e => e.OnTerminate(), Logger);
            shellEvents.Invoke(e => e.OnTerminate(), Logger);
            // From now on, cant access the container
            _container.Dispose();
        }

        public static IXNuvemHost Start(IContainer container, IAppBuilder app)
        {
            if (_currentHost == null) _currentHost = new XNuvemHost(container);

            // Application_End
            var properties = new AppProperties(app.Properties);
            var token = properties.OnAppDisposing;
            if (token != CancellationToken.None)
                token.Register(() => { Current.OnTerminate(); });

            _currentHost.OnStart();

            return _currentHost;
        }
    }
}