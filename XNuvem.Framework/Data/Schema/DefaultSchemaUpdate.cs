using System;
using NHibernate.Tool.hbm2ddl;
using XNuvem.Data.Providers;
using XNuvem.Environment;
using XNuvem.Environment.Configuration;
using XNuvem.Logging;

namespace XNuvem.Data.Schema
{
    public class DefaultSchemaUpdate : ISchemaUpdate
    {
        private readonly IDataServiceProvider _dataServiceProvider;
        private readonly IServiceContext _serviceContext;
        private readonly ISessionConfigurationCache _sessionConfigurationCache;
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly ITransactionManager _transactionManager;

        public DefaultSchemaUpdate(
            IShellSettingsManager shellSettingsManager,
            ISessionConfigurationCache sessionConfigurationCache,
            IDataServiceProvider dataServiceProvider,
            IServiceContext serviceContext,
            ITransactionManager transactionManager)
        {
            _shellSettingsManager = shellSettingsManager;
            _sessionConfigurationCache = sessionConfigurationCache;
            _dataServiceProvider = dataServiceProvider;
            _serviceContext = serviceContext;
            _transactionManager = transactionManager;
        }

        public ILogger Logger { get; set; }

        public bool HasUpdates()
        {
            var settings = _shellSettingsManager.GetSettings();
            if (string.IsNullOrEmpty(settings.ConnectionSettings.DataConnectionString)) return true;
            var sessionFactoryParameters = GetSessionFactoryParameters(settings);
            var cfg = _dataServiceProvider.BuildConfiguration(sessionFactoryParameters);
            var validator = new SchemaValidator(cfg);
            try
            {
                validator.Validate();
                return false;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Erro ao validar o schema");
            }
            return true; // Se chegou até aqui o schema é inválido
        }

        public void CreateDatabase()
        {
            Logger.Warning("Start creating database");
            _sessionConfigurationCache.InvalidateCache();
            var settings = _shellSettingsManager.GetSettings();
            var sessionFactoryParameters = GetSessionFactoryParameters(settings);
            var cfg = _dataServiceProvider.BuildConfiguration(sessionFactoryParameters);
            var se = new SchemaExport(cfg);
            se.Create(false, true);
            settings.ConnectionSettings.UpdatedAt = DateTime.Now;
            settings.ConnectionSettings.CreatedAt = DateTime.Now;
            _shellSettingsManager.StoreSettings(settings);
            Logger.Warning("End create database");
            var version = typeof(DefaultSchemaUpdate).Assembly.GetName().Version;
            var migrationHistory = new MigrationHistory
            {
                Name = "XNuvem.Framework",
                Version = version.ToString(),
                LongVersion = version.Major * 100 + version.Minor,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _transactionManager.RequireNew();
            var migrationHistoryRepository = _serviceContext.Resolve<IRepository<MigrationHistory>>();
            migrationHistoryRepository.Create(migrationHistory);
        }

        public void UpdateDatabse()
        {
            Logger.Warning("Start updating database");
            _sessionConfigurationCache.InvalidateCache();
            var settings = _shellSettingsManager.GetSettings();
            var sessionFactoryParameters = GetSessionFactoryParameters(settings);
            var cfg = _dataServiceProvider.BuildConfiguration(sessionFactoryParameters);
            var su = new SchemaUpdate(cfg);
            su.Execute(false, true);
            settings.ConnectionSettings.UpdatedAt = DateTime.Now;
            _shellSettingsManager.StoreSettings(settings);
            Logger.Warning("End update database");
        }

        private SessionFactoryParameters GetSessionFactoryParameters(ShellSettings settings)
        {
            return new SessionFactoryParameters
            {
                ConnectionString = settings.ConnectionSettings.DataConnectionString,
                Provider = settings.ConnectionSettings.DataProvider,
                CreateDatabase = false,
                Entities = settings.Entities,
                EntityMaps = settings.EntityMaps
            };
        }
    }
}