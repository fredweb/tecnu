/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using System.Collections.Generic;

namespace XNuvem.UI.Navigation
{
    public class MenuBuilder
    {
        private readonly Dictionary<string, MenuEntry> _menuItems;

        public MenuBuilder()
        {
            _menuItems = new Dictionary<string, MenuEntry>();
            RootMenu = new MenuEntry
            {
                Position = ""
            };
            _menuItems.Add("", RootMenu);
        }

        public MenuEntry RootMenu { get; }

        public MenuEntry Add(MenuEntry menu)
        {
            _menuItems.Add(menu.Position, menu);
            return menu;
        }

        public MenuEntry Build()
        {
            foreach (var menu in _menuItems.Values)
            {
                var parentPosition = menu.Father;
                MenuEntry parentMenu = null;
                if (_menuItems.TryGetValue(parentPosition, out parentMenu))
                    if (parentMenu.Position != menu.Position) parentMenu.Submenu.Add(menu);
            }
            return RootMenu;
        }
    }
}