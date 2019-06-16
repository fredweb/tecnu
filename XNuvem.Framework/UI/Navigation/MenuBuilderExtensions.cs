using System.Web.Routing;

namespace XNuvem.UI.Navigation
{
    public static class MenuBuilderExtensions
    {
        public static MenuEntry AddAction(this MenuBuilder _this, string position, string title, string actionName,
            string controllerName, object routeValues)
        {
            var menuItem = new MenuEntry
            {
                Type = MenuType.String,
                Title = title,
                Position = position,
                ActionName = actionName,
                ControllerName = controllerName,
                RouteValues = new RouteValueDictionary(routeValues)
            };

            return _this.Add(menuItem);
        }

        public static MenuEntry AddAction(this MenuBuilder _this, string position, string title, string actionName,
            string controllerName, string claimPermission, object routeValues)
        {
            var menuItem = new MenuEntry
            {
                Type = MenuType.String,
                Title = title,
                Position = position,
                ActionName = actionName,
                ControllerName = controllerName,
                RouteValues = new RouteValueDictionary(routeValues),
                Permission = claimPermission
            };

            return _this.Add(menuItem);
        }

        public static MenuEntry AddSeparator(this MenuBuilder _this, string position)
        {
            var menuItem = new MenuEntry
            {
                Type = MenuType.Separator,
                Position = position
            };

            return _this.Add(menuItem);
        }

        public static MenuEntry AddLabel(this MenuBuilder _this, string position, string title)
        {
            var menuItem = new MenuEntry
            {
                Type = MenuType.String,
                Position = position,
                Title = title
            };

            return _this.Add(menuItem);
        }

        public static MenuEntry AddGroup(this MenuBuilder _this, string position, string title)
        {
            var menuItem = new MenuEntry
            {
                Type = MenuType.Group,
                Title = title,
                Position = position
            };

            return _this.Add(menuItem);
        }

        public static MenuEntry AddGroup(this MenuBuilder _this, string position, string title, string claimPermission)
        {
            var menuItem = new MenuEntry
            {
                Type = MenuType.Group,
                Title = title,
                Position = position,
                Permission = claimPermission
            };

            return _this.Add(menuItem);
        }
    }
}