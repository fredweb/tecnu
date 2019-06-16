/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace XNuvem.UI.Navigation
{
    public enum MenuType
    {
        String,
        Group,
        Separator
    }

    public class MenuEntry
    {
        public MenuEntry()
        {
            Submenu = new List<MenuEntry>();
        }

        public string Permission { get; set; }
        public string Icon { get; set; }
        public MenuType Type { get; set; }
        public string Position { get; set; }

        public int Level => GetLevelFromPosition();

        public string Father => GetFatherFromPosition();

        public int Order { get; set; }
        public string Title { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
        public IList<MenuEntry> Submenu { get; }
        public string ImageUrl { get; set; }

        public IEnumerable<MenuEntry> SubmenuOrdered
        {
            get { return Submenu.OrderBy(m => m.Order).ThenBy(m => m.Level).ToList(); }
        }

        internal int GetLevelFromPosition()
        {
            var strLastPosition = string.IsNullOrEmpty(Position)
                ? Position
                : Position.Split('.').Reverse().FirstOrDefault();
            var result = int.MaxValue;
            if (!int.TryParse(strLastPosition, out result))
                return int.MaxValue;
            return result;
        }

        internal string GetFatherFromPosition()
        {
            return string.IsNullOrEmpty(Position) || !Position.Contains(".")
                ? ""
                : Position.Substring(0, Position.LastIndexOf('.'));
        }

        /// <summary>
        ///     Return a list of MenuEntry ordered by position
        /// </summary>
        /// <returns>A list of MenuEntry</returns>
        public IEnumerable<MenuEntry> Transverse(bool includeRoot = false)
        {
            var stack = new Stack<MenuEntry>();
            if (includeRoot)
            {
                stack.Push(this);
            }
            else
            {
                var roots = Submenu.OrderByDescending(m => m.Level).ToList();
                foreach (var item in roots) stack.Push(item);
            }
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                var ordered = next.Submenu.OrderByDescending(m => m.Level).ToList();
                foreach (var child in ordered) stack.Push(child);
            }
        }
    }
}