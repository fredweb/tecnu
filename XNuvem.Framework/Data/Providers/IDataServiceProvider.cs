/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;

namespace XNuvem.Data.Providers
{
    public interface IDataServiceProvider : ITransientDependency
    {
        Configuration BuildConfiguration(SessionFactoryParameters sessionFactoryParameters);
        IPersistenceConfigurer GetPersistenceConfigurer(SessionFactoryParameters sessionFactoryParameters);
    }
}