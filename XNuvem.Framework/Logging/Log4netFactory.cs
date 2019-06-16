/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 *
 * Este código faz parte do Orchard e é livre para distribuição
 * 
 * 
/****************************************************************************************/

using System;
using System.Configuration;
using System.IO;
using System.Web;
using log4net;
using log4net.Config;

namespace XNuvem.Logging
{
    public class Log4netFactory : ILoggerFactory
    {
        private static bool _isFileWatched;

        public ILogger CreateLogger(Type type)
        {
            if (!_isFileWatched)
            {
                var configInfo = GetFileInfo();
                if (null != configInfo)
                {
                    XmlConfigurator.ConfigureAndWatch(configInfo);
                    _isFileWatched = true;
                }
            }
            return new Log4netLogger(LogManager.GetLogger(type), this);
        }

        public FileInfo GetFileInfo()
        {
            var fileName = ConfigurationManager.AppSettings["log4net.Config"];
            if (string.IsNullOrEmpty(fileName))
                return null;
            var currentPath = "";
            if (null != HttpContext.Current) currentPath = HttpContext.Current.Server.MapPath("~/");
            else currentPath = Directory.GetCurrentDirectory();

            var fileFullName = Path.Combine(currentPath, fileName);
            if (File.Exists(fileFullName)) return new FileInfo(fileFullName);
            return null;
        }
    }
}