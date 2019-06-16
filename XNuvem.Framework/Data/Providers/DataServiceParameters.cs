/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using System;
using System.Collections.Generic;

namespace XNuvem.Data.Providers
{
    public class DataServiceParameters
    {
        public string Provider { get; set; }
        public string DataFolder { get; set; }
        public string ConnectionString { get; set; }
        public IList<Type> Entities { get; set; }
        public IList<Type> EntityMaps { get; set; }
    }
}