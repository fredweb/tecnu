using System.Collections.Generic;
using XNuvem.Security.Permissions;

namespace XNuvem.Security
{
    public class DefaultPermissions : IPermissionProvider
    {
        public static readonly Permission AccessConfiguration = new Permission
        {
            Name = "http://xnuvem/configuration",
            Position = 1,
            Description = "Configuração",
            Summary = "Configurações gerais do sistema"
        };

        public static readonly Permission AccessGeneral = new Permission
        {
            Name = "http://xnuvem/configuration/general",
            Parent = "http://xnuvem/configuration",
            Position = 1,
            Description = "Geral",
            Summary = "Configurações gerais"
        };

        public static readonly Permission AccessUserList = new Permission
        {
            Name = "http://xnuvem/configuration/general/user-list",
            Parent = "http://xnuvem/configuration/general",
            Position = 1,
            Description = "Lista de usuários",
            Summary = "Lista de usuários"
        };

        public static readonly Permission AccessUserEdit = new Permission
        {
            Name = "http://xnuvem/configuration/general/user-edit",
            Parent = "http://xnuvem/configuration/general",
            Position = 2,
            Description = "Cadastro de usuários",
            Summary = "Cadastro de usuários"
        };

        public static readonly Permission AccessAuthorization = new Permission
        {
            Name = "http://xnuvem/configuration/general/authorizatios",
            Parent = "http://xnuvem/configuration/general",
            Position = 3,
            Description = "Autorizações",
            Summary = "Autorizações de usuários"
        };

        public static readonly Permission AccessGlobalConfiguration = new Permission
        {
            Name = "http://xnuvem/configuration/general/global",
            Parent = "http://xnuvem/configuration/general",
            Position = 4,
            Description = "Configurações gerais",
            Summary = "Configurações gerais"
        };

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                AccessConfiguration, AccessGeneral, AccessUserList, AccessUserEdit, AccessAuthorization,
                AccessGlobalConfiguration
            };
        }
    }
}