/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using XNuvem.Data.Providers;
using XNuvem.Environment.Configuration;
using XNuvem.Logging;
using XNuvem.Utility.Extensions;

namespace XNuvem.Data
{
    public class SessionFactoryHolder : ISessionFactoryHolder
    {
        private readonly Func<IEnumerable<ISessionConfigurationEvents>> _configurers;
        private readonly IDataServiceProvider _dataServiceProvider;
        private readonly IShellSettingsManager _shellSettingsManager;
        private Configuration _configuration;

        private ISessionFactory _sessionFactory;

        public SessionFactoryHolder(
            IShellSettingsManager shellSettingsManager,
            IDataServiceProvider dataServieProvider,
            Func<IEnumerable<ISessionConfigurationEvents>> configurers)
        {
            _shellSettingsManager = shellSettingsManager;
            _dataServiceProvider = dataServieProvider;
            _configurers = configurers;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public ISessionFactory GetSessionFactory()
        {
            lock (this)
            {
                if (_sessionFactory == null) _sessionFactory = BuildSessionFactory();
            }
            return _sessionFactory;
        }

        public Configuration GetConfiguration()
        {
            lock (this)
            {
                if (_configuration == null) _configuration = BuildConfiguration();
            }
            return _configuration;
        }


        private ISessionFactory BuildSessionFactory()
        {
            Logger.Debug("Building session factory");

            var config = GetConfiguration();
            var result = config.BuildSessionFactory();
            Logger.Debug("Done building session factory");
            return result;
        }

        private Configuration BuildConfiguration()
        {
            Logger.Debug("Building configuration");
            var parameters = GetSessionFactoryParameters();

            // Cache configuration must be here becouse throw error if not
            var config = _dataServiceProvider.BuildConfiguration(parameters)
                .Cache(c => c.UseQueryCache = false);

            #region NH specific optimization

            // cannot be done in fluent config
            // the IsSelectable = false prevents unused ContentPartRecord proxies from being created
            // for each ContentItemRecord or ContentItemVersionRecord.
            // done for perf reasons - has no other side-effect

            //foreach (var persistentClass in config.ClassMappings) {
            //    if (persistentClass.EntityName.StartsWith("Orchard.ContentManagement.Records.")) {
            //        foreach (var property in persistentClass.PropertyIterator) {
            //            if (property.Name.EndsWith("Record") && !property.IsBasicPropertyAccessor) {
            //                property.IsSelectable = false;
            //            }
            //        }
            //    }
            //}

            #endregion

            parameters.Configurers.Invoke(c => c.Finished(config), Logger);

            Logger.Debug("Done Building configuration");
            return config;
        }

        private SessionFactoryParameters GetSessionFactoryParameters()
        {
            var settings = _shellSettingsManager.GetSettings();

            return new SessionFactoryParameters
            {
                Configurers = _configurers(),
                Provider = settings.ConnectionSettings.DataProvider,
                DataFolder = "Site",
                ConnectionString = settings.ConnectionSettings.DataConnectionString,
                Entities = settings.Entities,
                EntityMaps = settings.EntityMaps
            };
        }
    }
}