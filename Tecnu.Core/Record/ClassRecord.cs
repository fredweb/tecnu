using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Tecnu.Core.Record.Base;
using XNuvem.Data;

namespace Tecnu.Core.Record
{
    public class ClassRecord : RecordBase
    {
        public virtual DateTime Date { get; set; }
        public virtual DateTime Schedule { get; set; }
        public virtual TeacherRecord Teacher { get; set; }
        public virtual PeriodRecord Period { get; set; }
        [ScriptIgnore]
        public virtual ICollection<StudentRecord> Students { get; set; }
    }

    public class ClassMap : EntityMap<ClassRecord>
    {
        public ClassMap()
        {
            Table("CLASSE");
            Id(k => k.Id).Column("ID").GeneratedBy.Increment();
            Map(m => m.Date).Column("DTAULA").Not.Nullable();
            Map(m => m.Schedule).Column("HORARIO").Not.Nullable();
            Map(m => m.When).Column("DTCADASTRO").Not.Nullable();
            Map(m => m.Update).Column("DTATUALIZACAO").Not.Nullable();

            References(w => w.Teacher).Column("SQPROFESSOR").ForeignKey("FKPROFESSOR").Not.Nullable();
            References(w => w.Period).Column("SQPERIODO").ForeignKey("FKPERIODO").Not.Nullable();
            HasManyToMany(w => w.Students).Table("ClasseAlunos").ParentKeyColumn("SQCLASSE").ChildKeyColumn("SQALUNO").Cascade.Delete().Inverse();
        }
    }
}