/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;
using XNuvem.Data.Conventions;
using XNuvem.Environment.Configuration;
using XNuvem.Logging;
using XNuvem.Utility.Extensions;

namespace XNuvem.Data.Providers
{
    public class DefaultDataServiceProvider : IDataServiceProvider
    {
        private readonly ISessionConfigurationCache _configurationCache;
        private readonly IShellSettingsManager _shellSettingsManager;

        public DefaultDataServiceProvider(IShellSettingsManager shellSettingsManager,
            ISessionConfigurationCache configurationCache)
        {
            _shellSettingsManager = shellSettingsManager;
            _configurationCache = configurationCache;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public Configuration BuildConfiguration(SessionFactoryParameters parameters)
        {
            Logger.Debug("Build database configuration");
            // Try get configuration from the cache
            var config = _configurationCache.GetConfiguration();
            if (config != null) return config;

            var persistence = GetPersistenceConfigurer(parameters);

            config = Fluently.Configure()
                .Database(persistence)
                .Mappings(m => BuildMappings(m, parameters))
                .ExposeConfiguration(cfg =>
                {
                    cfg
                        .SetProperty(NHibernate.Cfg.Environment.FormatSql, bool.FalseString)
                        .SetProperty(NHibernate.Cfg.Environment.GenerateStatistics, bool.FalseString)
                        .SetProperty(NHibernate.Cfg.Environment.Hbm2ddlKeyWords, Hbm2DDLKeyWords.None.ToString())
                        .SetProperty(NHibernate.Cfg.Environment.PropertyBytecodeProvider, "lcg")
                        .SetProperty(NHibernate.Cfg.Environment.PropertyUseReflectionOptimizer, bool.TrueString)
                        .SetProperty(NHibernate.Cfg.Environment.QueryStartupChecking, bool.FalseString)
                        //TODO: Mudar para false em release mode (ShowSql)
                        .SetProperty(NHibernate.Cfg.Environment.ShowSql, bool.TrueString)
                        .SetProperty(NHibernate.Cfg.Environment.StatementFetchSize, "100")
                        .SetProperty(NHibernate.Cfg.Environment.UseProxyValidator, bool.FalseString)
                        .SetProperty(NHibernate.Cfg.Environment.UseSqlComments, bool.FalseString)
                        .SetProperty(NHibernate.Cfg.Environment.WrapResultSets, bool.TrueString)
                        .SetProperty(NHibernate.Cfg.Environment.BatchSize, "256")
                        ;

                    cfg.EventListeners.LoadEventListeners = new ILoadEventListener[] {new XNuvemLoadEventListener()};
                    cfg.EventListeners.PostLoadEventListeners = new IPostLoadEventListener[0];
                    cfg.EventListeners.PreLoadEventListeners = new IPreLoadEventListener[0];

                    // don't enable PrepareSql by default as it breaks on SqlCe
                    cfg.SetProperty(NHibernate.Cfg.Environment.PrepareSql, bool.TrueString);

                    parameters.Configurers.Invoke(c => c.Building(cfg), Logger);
                }).BuildConfiguration();

            // Store configuration on cache
            _configurationCache.SetConfiguration(config);

            Logger.Debug("Done build database configuration");
            return config;
        }

        public IPersistenceConfigurer GetPersistenceConfigurer(SessionFactoryParameters sessionFactoryParameters)
        {
            if (string.IsNullOrEmpty(sessionFactoryParameters.ConnectionString))
                throw new ArgumentException("The connection string is empty");

            var persistence = MsSqlConfiguration.MsSql2012;
            persistence = persistence.ConnectionString(sessionFactoryParameters.ConnectionString);

            // use MsSql2012Dialect to be compatible with Azure
            persistence = persistence.Dialect<MsSql2012Dialect>();

            return persistence;
        }

        internal void BuildMappings(MappingConfiguration mapping, DataServiceParameters parameters)
        {
            foreach (var map in parameters.EntityMaps) mapping.FluentMappings.Add(map);
            mapping.FluentMappings.Conventions.Add(typeof(XNuvemForeinKeyConvention));
        }

        #region LoadEventListener class...

        [Serializable]
        private class XNuvemLoadEventListener : DefaultLoadEventListener, ILoadEventListener
        {
            public new void OnLoad(LoadEvent @event, LoadType loadType)
            {
                var source = (ISessionImplementor) @event.Session;
                IEntityPersister entityPersister;
                if (@event.InstanceToLoad != null)
                {
                    entityPersister = source.GetEntityPersister(null, @event.InstanceToLoad);
                    @event.EntityClassName = @event.InstanceToLoad.GetType().FullName;
                }
                else
                {
                    entityPersister = source.Factory.GetEntityPersister(@event.EntityClassName);
                }
                if (entityPersister == null)
                    throw new HibernateException("Unable to locate persister: " + @event.EntityClassName);

                //a hack to handle unused ContentPartRecord proxies on ContentItemRecord or ContentItemVersionRecord.
                //I don't know why it actually works, or how to do it right

                //if (!entityPersister.IdentifierType.IsComponentType)
                //{
                //    Type returnedClass = entityPersister.IdentifierType.ReturnedClass;
                //    if (returnedClass != null && !returnedClass.IsInstanceOfType(@event.EntityId))
                //        throw new TypeMismatchException(string.Concat(new object[4]
                //    {
                //      (object) "Provided id of the wrong type. Expected: ",
                //      (object) returnedClass,
                //      (object) ", got ",
                //      (object) @event.EntityId.GetType()
                //    }));
                //}

                var keyToLoad = new EntityKey(@event.EntityId, entityPersister, source.EntityMode);

                if (loadType.IsNakedEntityReturned) @event.Result = Load(@event, entityPersister, keyToLoad, loadType);
                else if (@event.LockMode == LockMode.None)
                    @event.Result = ProxyOrLoad(@event, entityPersister, keyToLoad, loadType);
                else @event.Result = LockAndLoad(@event, entityPersister, keyToLoad, loadType, source);
            }
        }

        #endregion
    }
}