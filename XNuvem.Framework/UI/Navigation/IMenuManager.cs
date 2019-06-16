using System.Collections.Generic;

namespace XNuvem.UI.Navigation
{
    public interface IMenuManager
    {
        void BuildMenu(IEnumerable<IMenuProvider> providers);
        IEnumerable<MenuEntry> GetMenuAsList(bool includeRoot);
        MenuEntry GetRootMenu();
    }
}