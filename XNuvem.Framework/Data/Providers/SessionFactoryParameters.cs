/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using System.Collections.Generic;
using System.Linq;

namespace XNuvem.Data.Providers
{
    public class SessionFactoryParameters : DataServiceParameters
    {
        public SessionFactoryParameters()
        {
            Configurers = Enumerable.Empty<ISessionConfigurationEvents>();
        }

        public IEnumerable<ISessionConfigurationEvents> Configurers { get; set; }
        public bool CreateDatabase { get; set; }
    }
}