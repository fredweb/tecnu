using Tecnu.Core.Record.Base;
using XNuvem.Data;

namespace Tecnu.Core.Record
{
    public class PeriodRecord : RecordBase
    {
        public virtual string Name { get; set; }
    }

    public class PeriodMap : EntityMap<PeriodRecord>
    {
        public PeriodMap()
        {
            Table("PERIODO");
            Id(k => k.Id).Column("ID").GeneratedBy.Increment();
            Map(m => m.Name).Column("NNPERIODO").Length(20).Unique().Not.Nullable();
            Map(m => m.When).Column("DTCADASTRO").Not.Nullable();
            Map(m => m.Update).Column("DTATUALIZACAO").Not.Nullable();
        }
    }
}