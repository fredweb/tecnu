using System;

namespace XNuvem.Mvc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AdminAttribute : Attribute
    {
        public AdminAttribute(string areaName, string groupName)
        {
            AreaName = areaName;
            GroupName = groupName;
        }

        public string AreaName { get; protected set; }
        public string GroupName { get; protected set; }
    }
}