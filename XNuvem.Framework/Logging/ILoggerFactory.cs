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

namespace XNuvem.Logging
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger(Type type);
    }
}