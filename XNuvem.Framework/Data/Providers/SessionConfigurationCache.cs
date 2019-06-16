/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cfg;
using NHibernate.Type;
using XNuvem.Environment.Configuration;
using XNuvem.Exceptions;
using XNuvem.FileSystems.AppData;
using XNuvem.Logging;
using XNuvem.Utility;
using XNuvem.Utility.Extensions;

namespace XNuvem.Data.Providers
{
    public class SessionConfigurationCache : ISessionConfigurationCache
    {
        private const string CSMapFileName = "mappings.bin";
        private static readonly object _syncLock = new object();
        private readonly IAppDataFolder _appDataFolder;
        private readonly IEnumerable<ISessionConfigurationEvents> _configurers;
        private readonly IShellSettingsManager _shellSettingsManager;
        private ConfigurationCache _currentConfig;

        public SessionConfigurationCache(IShellSettingsManager shellSettingsManager, IAppDataFolder appDataFolder,
            IEnumerable<ISessionConfigurationEvents> configurers)
        {
            _shellSettingsManager = shellSettingsManager;
            _appDataFolder = appDataFolder;
            _configurers = configurers;
            _currentConfig = null;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
        public bool Disabled { get; set; }

        public Configuration GetConfiguration()
        {
            lock (this)
            {
                if (Disabled) return null;
                Logger.Debug("Get configuration cache");

                var hash = ComputeHash().Value;

                // if the current configuration is unchanged, return it
                if (_currentConfig != null && _currentConfig.Hash == hash) return _currentConfig.Configuration;

                // Return previous configuration if it exists and has the same hash as
                // the current blueprint.
                var previousConfig = ReadConfiguration(hash);
                if (previousConfig != null)
                {
                    _currentConfig = previousConfig;
                    return previousConfig.Configuration;
                }
            }

            //If reach here, dont have any configuration
            return null;
        }

        public void SetConfiguration(Configuration config)
        {
            Logger.Debug("Setting configuration cache");
            lock (_syncLock)
            {
                if (Disabled) return;

                var hash = ComputeHash().Value;

                // Create cache and persist it
                _currentConfig = new ConfigurationCache
                {
                    Hash = hash,
                    Configuration = config
                };

                StoreConfiguration(_currentConfig);
            }
        }

        public void InvalidateCache()
        {
            lock (_syncLock)
            {
                _appDataFolder.DeleteFile(GetPathName());
            }
        }

        private void StoreConfiguration(ConfigurationCache cache)
        {
            var pathName = GetPathName();

            try
            {
                var formatter = new BinaryFormatter();
                using (var stream = _appDataFolder.CreateFile(pathName))
                {
                    formatter.Serialize(stream, cache.Hash);
                    formatter.Serialize(stream, cache.Configuration);
                }
            }
            catch (SerializationException ex)
            {
                //Note: This can happen when multiple processes/AppDomains try to save
                //      the cached configuration at the same time. Only one concurrent
                //      writer will win, and it's harmless for the other ones to fail.
                for (Exception scan = ex; scan != null; scan = scan.InnerException)
                    Logger.Warning("Error storing new NHibernate cache configuration: {0}", scan.Message);
            }
        }

        private ConfigurationCache ReadConfiguration(string hash)
        {
            var pathName = GetPathName();

            if (!_appDataFolder.FileExists(pathName))
                return null;

            try
            {
                var formatter = new BinaryFormatter();
                using (var stream = _appDataFolder.OpenFile(pathName))
                {
                    // if the stream is empty, stop here
                    if (stream.Length == 0) return null;

                    var oldHash = (string) formatter.Deserialize(stream);
                    if (hash != oldHash)
                    {
                        Logger.Information(
                            "The cached NHibernate configuration is out of date. A new one will be re-generated.");
                        return null;
                    }

                    var oldConfig = (Configuration) formatter.Deserialize(stream);

                    return new ConfigurationCache
                    {
                        Hash = oldHash,
                        Configuration = oldConfig
                    };
                }
            }
            catch (Exception ex)
            {
                if (ex.IsFatal()) throw;
                for (var scan = ex; scan != null; scan = scan.InnerException)
                    Logger.Warning("Error reading the cached NHibernate configuration: {0}", scan.Message);
                Logger.Information("A new one will be re-generated.");
                return null;
            }
        }

        private Hash ComputeHash()
        {
            var hash = new Hash();

            // Shell settings physical location
            //   The nhibernate configuration stores the physical path to the SqlCe database
            //   so we need to include the physical location as part of the hash key, so that
            //   xcopy migrations work as expected.
            var pathName = GetPathName();
            hash.AddString(_appDataFolder.MapPath(pathName).ToLowerInvariant());

            // XNuvem version, to rebuild the mappings for each new version
            var xnuvemVersion =
                new AssemblyName(typeof(SessionConfigurationCache).Assembly.FullName).Version.ToString();
            hash.AddString(xnuvemVersion);

            // Date and time of the current assembly
            var fileInfo = new FileInfo(typeof(SessionConfigurationCache).Assembly.Location);
            hash.AddDateTime(fileInfo.CreationTime);

            var settings = _shellSettingsManager.GetSettings();
            // Shell settings data
            hash.AddString(settings.ConnectionSettings.DataConnectionString);
            hash.AddString(settings.ConnectionSettings.DataProvider);

            // Assembly names, record names and property names
            foreach (var recordType in settings.Entities)
            {
                hash.AddTypeReference(recordType);

                if (recordType.BaseType != null)
                    hash.AddTypeReference(recordType.BaseType);

                foreach (var property in recordType.GetProperties(
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
                {
                    hash.AddString(property.Name);
                    hash.AddTypeReference(property.PropertyType);

                    foreach (var attr in property.GetCustomAttributesData())
                        hash.AddTypeReference(attr.Constructor.DeclaringType);
                }
            }

            _configurers.Invoke(c => c.ComputingHash(hash), Logger);

            return hash;
        }

        private string GetPathName()
        {
            return CSMapFileName;
        }

        private class ConfigurationCache
        {
            public string Hash { get; set; }
            public Configuration Configuration { get; set; }
        }
    }
}