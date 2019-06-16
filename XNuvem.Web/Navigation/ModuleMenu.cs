using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using XNuvem.Security;
using XNuvem.UI.Navigation;

namespace XNuvem.Web.Navigation
{
    public class ModuleMenu : IMenuProvider
    {
        public void BuildMenu(MenuBuilder builder) {
            builder.AddGroup("1", "Configurações");
            builder.AddGroup("1.2", "Segurança");
            builder.AddAction("1.2.1", "Alterar senha", "ChangePassword", "SettingsUser", new { area = "" });
            builder.AddAction("1.2.2", "Usuário", "UserEdit", "SettingsUser", DefaultClaims.Administrator.Value, new { area = "" });
            builder.AddAction("1.2.3", "Lista de usuários", "UserList", "SettingsUser", DefaultClaims.Administrator.Value, new { area = "" });

            builder.AddSeparator("90000");
            builder.Add(new MenuEntry {
                Position = "90001",
                Title = "Sair",
                Icon = "fa-circle-o text-red",
                ActionName = "SignOut",
                ControllerName = "Account",
                RouteValues = new RouteValueDictionary(new { area = "" })
            });
        }
    }
}