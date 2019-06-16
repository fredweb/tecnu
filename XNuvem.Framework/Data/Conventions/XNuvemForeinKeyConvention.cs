using System;
using FluentNHibernate;
using FluentNHibernate.Conventions;

namespace XNuvem.Data.Conventions
{
    public class XNuvemForeinKeyConvention : ForeignKeyConvention
    {
        protected override string GetKeyName(Member property, Type type)
        {
            if (property == null) return type.Name + "Code";

            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(long))
                return type.Name + "Entry";

            return type.Name + "Code";
        }
    }
}