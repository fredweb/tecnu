/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using NHibernate.Cfg;

namespace XNuvem.Data.Providers
{
    public interface ISessionConfigurationCache
    {
        Configuration GetConfiguration();

        void SetConfiguration(Configuration config);

        void InvalidateCache();
    }
}