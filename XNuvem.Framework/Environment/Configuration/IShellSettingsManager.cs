/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using System.Collections.Generic;
using XNuvem.Security.Permissions;

namespace XNuvem.Environment.Configuration
{
    public interface IShellSettingsManager
    {
        ShellSettings GetSettings();
        void StoreSettings(ShellSettings settings);
        IEnumerable<Permission> GetPermissions();

        bool HasConfigurationFile();
    }
}