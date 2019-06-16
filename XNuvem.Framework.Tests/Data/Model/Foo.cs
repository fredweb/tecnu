using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNuvem.Tests.Data.Model
{
    public class Fool
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class FoolMap : ClassMap<Fool>
    {
        public FoolMap() {
            Table("Fools");

            Id(x => x.Id).GeneratedBy.Identity();
            
            Map(x => x.Name);
        }
    }
}
