/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using XNuvem.Data;
using XNuvem.FileSystems.AppData;
using XNuvem.Logging;
using XNuvem.Security.Permissions;

namespace XNuvem.Environment.Configuration
{
    public class ShellSettingsManager : IShellSettingsManager
    {
        private const string SettingsFile = "xnuvem-settings.xml";
        private const string GeneralSettingsFile = "xnuvem-app-settings.xml";
        private const string CSConnectionStringKey = "XNuvem";
        private readonly IAppDataFolder _appDataFolder;
        private ShellSettings _settings;

        public ShellSettingsManager(IAppDataFolder appDataFolder)
        {
            _settings = null;
            _appDataFolder = appDataFolder;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public ShellSettings GetSettings()
        {
            lock (this)
            {
                if (_settings != null) return _settings;

                Logger.Debug("Loading shell configurations");

                var connectionSettings = GetConnectionSettings();
                var entitiesMaps = GetShellEntityMaps();
                var entities = GetShellEntities(entitiesMaps);
                var generalSettings = GetGeneralSettings();

                _settings = new ShellSettings
                {
                    ConnectionSettings = connectionSettings,
                    Entities = entities,
                    EntityMaps = entitiesMaps
                };

                // Load all general settings
                generalSettings.FlushToDictionary(_settings.GeneralSettings);
            }
            return _settings;
        }

        public IEnumerable<Permission> GetPermissions()
        {
            var permissionProviders = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => IsTypeOf<IPermissionProvider>(t))
                .ToList();

            var list = new List<Permission>();
            foreach (var provider in permissionProviders)
            {
                var providerInstance = (IPermissionProvider) Activator.CreateInstance(provider);
                list.AddRange(providerInstance.GetPermissions());
            }

            return list.ToArray();
        }

        public void StoreSettings(ShellSettings settings)
        {
            Logger.Debug("Storing configuration settings.");
            lock (this)
            {
                var connectionSettings = settings.ConnectionSettings;
                using (var stream = _appDataFolder.CreateFile(SettingsFile))
                {
                    var serializer = new XmlSerializer(typeof(ConnectionSettings));
                    serializer.Serialize(stream, connectionSettings);
                }
                var generalSettings = new GeneralSettingsHelper(settings.GeneralSettings);
                using (var stream = _appDataFolder.CreateFile(GeneralSettingsFile))
                {
                    var serializer = new XmlSerializer(typeof(GeneralSettingsHelper));
                    serializer.Serialize(stream, generalSettings);
                }
                _settings = null; // force reload settings
            }
        }

        public bool HasConfigurationFile()
        {
            return _appDataFolder.FileExists(SettingsFile);
        }

        internal ConnectionSettings GetConnectionSettings()
        {
            ConnectionSettings settings = null;
            if (_appDataFolder.FileExists(SettingsFile))
                using (var stream = _appDataFolder.OpenFile(SettingsFile))
                {
                    var serializer = new XmlSerializer(typeof(ConnectionSettings));
                    settings = serializer.Deserialize(stream) as ConnectionSettings;
                }
            if (settings == null) settings = new ConnectionSettings();
            return settings;
        }

        internal GeneralSettingsHelper GetGeneralSettings()
        {
            GeneralSettingsHelper settings = null;
            if (_appDataFolder.FileExists(GeneralSettingsFile))
                using (var stream = _appDataFolder.OpenFile(GeneralSettingsFile))
                {
                    var serializer = new XmlSerializer(typeof(GeneralSettingsHelper));
                    settings = serializer.Deserialize(stream) as GeneralSettingsHelper;
                }
            if (settings == null) settings = new GeneralSettingsHelper();
            return settings;
        }

        internal IList<Type> GetShellEntityMaps()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                //.Where(a => a.FullName.StartsWith("XNuvem.")) //Aumenta a performance
                .Where(a => !a.IsDynamic && !a.GlobalAssemblyCache)
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => IsMappingOf<IEntityMap>(t))
                .ToList();
        }

        internal IList<Type> GetShellEntities(IList<Type> entityMaps)
        {
            //The Type.BaseType is a EntityMap<T>, return all types of T
            return entityMaps.Select(t => t.BaseType.GetGenericArguments().First()).ToList();
        }

        internal bool IsMappingOf<T>(Type type)
        {
            return !type.IsInterface && !type.IsGenericType && typeof(T).IsAssignableFrom(type);
        }

        internal bool IsTypeOf<T>(Type type)
        {
            return !type.IsInterface && typeof(T).IsAssignableFrom(type);
        }
    }
}