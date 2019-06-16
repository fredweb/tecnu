using System.Collections.Generic;
using XNuvem.Environment;
using XNuvem.Logging;

namespace XNuvem.UI.Navigation
{
    public class MenuBuilderEventHandler : IHostEvents
    {
        private readonly IMenuManager _menuManager;
        private readonly IEnumerable<IMenuProvider> _providers;

        public MenuBuilderEventHandler(IMenuManager menuManager, IEnumerable<IMenuProvider> providers)
        {
            _menuManager = menuManager;
            _providers = providers;
            Logger = NullLogger.Instance;
        }


        public ILogger Logger { get; set; }

        public void OnInitialize()
        {
            _menuManager.BuildMenu(_providers);
        }

        public void OnTerminate()
        {
        }
    }
}