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
using System.Globalization;
using log4net;
using log4net.Util;

namespace XNuvem.Logging
{
    public class Log4netLogger : ILogger
    {
        private Log4netFactory _factory;
        private readonly ILog _log;

        public Log4netLogger(ILog log, Log4netFactory factory)
        {
            _log = log;
            _factory = factory;
        }

        public bool IsEnabled(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return _log.IsDebugEnabled;
                case LogLevel.Error:
                    return _log.IsErrorEnabled;
                case LogLevel.Fatal:
                    return _log.IsFatalEnabled;
                case LogLevel.Information:
                    return _log.IsInfoEnabled;
                case LogLevel.Warning:
                    return _log.IsWarnEnabled;
            }
            return false;
        }

        public void Log(LogLevel level, Exception exception, string format, params object[] args)
        {
            if (IsEnabled(level))
                if (args == null)
                    switch (level)
                    {
                        case LogLevel.Debug:
                            _log.Debug(format, exception);
                            break;
                        case LogLevel.Error:
                            _log.Error(format, exception);
                            break;
                        case LogLevel.Fatal:
                            _log.Fatal(format, exception);
                            break;
                        case LogLevel.Information:
                            _log.Info(format, exception);
                            break;
                        case LogLevel.Warning:
                            _log.Warn(format, exception);
                            break;
                    }
                else
                    switch (level)
                    {
                        case LogLevel.Debug:
                            _log.Debug(new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                            break;
                        case LogLevel.Error:
                            _log.Error(new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                            break;
                        case LogLevel.Fatal:
                            _log.Fatal(new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                            break;
                        case LogLevel.Information:
                            _log.Info(new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                            break;
                        case LogLevel.Warning:
                            _log.Warn(new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                            break;
                    }
        }
    }
}