using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNuvem.Data;

namespace XNuvem.Tests.Data.Model
{
    public class Bar
    {
        public virtual string Name {get; set;}

        public virtual string Description { get; set; }
    }

    public class BarMap : EntityMap<Bar>
    {
        public BarMap() {
            Id(x => x.Name).Length(10);

            Map(x => x.Description).Length(100);
        }
    }
}
