using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XNuvem.Security;
using XNuvem.UI;
using XNuvem.UI.Navigation;

namespace XNuvem.Web.Models
{
    public class NavigationViewModel
    {
        public User CurrentUser { get; set; }
        public MenuEntry Root { get; set; }

        public NavigationViewModel() {
            Root = new MenuEntry();
        }

        public NavigationViewModel(MenuEntry rootMenu) {
            Root = rootMenu;
        }
    }
}