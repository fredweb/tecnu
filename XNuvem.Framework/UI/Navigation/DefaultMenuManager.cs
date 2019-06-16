using System.Collections.Generic;
using XNuvem.Logging;
using XNuvem.Utility.Extensions;

namespace XNuvem.UI.Navigation
{
    public class DefaultMenuManager : IMenuManager
    {
        private readonly MenuBuilder _menuBuilder;

        private MenuEntry _rootMenu;

        public DefaultMenuManager()
        {
            _menuBuilder = new MenuBuilder();
        }

        public ILogger Logger { get; set; }


        public void BuildMenu(IEnumerable<IMenuProvider> providers)
        {
            providers.Invoke(m => m.BuildMenu(_menuBuilder), Logger);
            _rootMenu = _menuBuilder.Build();
        }

        public IEnumerable<MenuEntry> GetMenuAsList(bool includeRoot)
        {
            return _rootMenu.Transverse(includeRoot);
        }

        public MenuEntry GetRootMenu()
        {
            return _rootMenu;
        }
    }
}