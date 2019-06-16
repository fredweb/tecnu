/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 *
 * 
/****************************************************************************************/


using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NHibernate.AspNet.Identity;
using XNuvem.Data;

namespace XNuvem.Security
{
    public class User : IdentityUser
    {
        public virtual string FullName { get; set; }

        public virtual int SlpCode { get; set; }
        public virtual string SlpName { get; set; }

        /// <summary>
        ///     Característica de items 60
        ///     Usado para empresa OK Alimentos
        /// </summary>
        public virtual string QryGroup60 { get; set; }

        /// <summary>
        ///     Característica de items 61
        ///     Usado para empresa X'TOSO
        /// </summary>
        public virtual string QryGroup61 { get; set; }

        /// <summary>
        ///     Característica de items 62
        ///     Usado para empresa Sabor da Bahia
        /// </summary>
        public virtual string QryGroup62 { get; set; }

        public async Task<ClaimsIdentity> CreateIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            var newIdentity = await manager.FindByNameAsync(UserName);
            return await manager.CreateIdentityAsync(newIdentity, authenticationType);
        }
    }

    public class UserMap : SubEntity<User>
    {
        public UserMap()
        {
            DiscriminatorValue("User");
            Map(x => x.FullName).Length(100);
            Map(x => x.SlpCode).Default("(-1)").Not.Nullable();
            Map(x => x.SlpName).Length(100);

            Map(x => x.QryGroup60).Length(1);
            Map(x => x.QryGroup61).Length(1);
            Map(x => x.QryGroup62).Length(1);
        }
    }
}