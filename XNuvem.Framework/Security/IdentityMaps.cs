/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 *
 * 
/****************************************************************************************/


using NHibernate.AspNet.Identity;
using XNuvem.Data;

namespace XNuvem.Security
{
    public class IdentityUserClaimMap : EntityMap<IdentityUserClaim>
    {
        public IdentityUserClaimMap()
        {
            Table("AspNetUserClaims");

            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.ClaimType).Length(255);

            Map(x => x.ClaimValue).Length(255);

            References(x => x.User);
        }
    }

    public class IdentityRoleMap : EntityMap<IdentityRole>
    {
        public IdentityRoleMap()
        {
            Table("AspNetRoles");

            Id(x => x.Id).Length(50).GeneratedBy.UuidHex("D");

            Map(x => x.Name).Length(255).Not.Nullable().Unique();

            HasManyToMany(x => x.Users)
                .Table("AspNetUserRoles")
                .Cascade.None()
                .Not.LazyLoad();
        }
    }

    public class IdentityUserMap : EntityMap<IdentityUser>
    {
        public IdentityUserMap()
        {
            Table("AspNetUsers");
            Id(x => x.Id).GeneratedBy.UuidHex("D").Length(50);

            DiscriminateSubClassesOnColumn("ClassType").Length(100);

            Map(x => x.AccessFailedCount);

            Map(x => x.Email).Length(255).Not.Nullable();

            Map(x => x.EmailConfirmed);

            Map(x => x.LockoutEnabled);

            Map(x => x.LockoutEndDateUtc);

            Map(x => x.PasswordHash).Length(550);

            Map(x => x.PhoneNumber).Length(100);

            Map(x => x.PhoneNumberConfirmed);

            Map(x => x.TwoFactorEnabled);

            Map(x => x.UserName).Length(100).Not.Nullable().Unique();

            Map(x => x.SecurityStamp).Length(50);

            HasMany(x => x.Claims)
                .Not.LazyLoad()
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.Logins)
                .Not.LazyLoad()
                .Cascade.AllDeleteOrphan()
                .Component(comp =>
                {
                    comp.Map(p => p.LoginProvider);
                    comp.Map(p => p.ProviderKey);
                });

            HasManyToMany(x => x.Roles)
                .Table("AspNetUserRoles")
                .Not.LazyLoad();
        }
    }
}