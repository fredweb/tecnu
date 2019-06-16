using System;
using Tecnu.Core.Record.Base;
using XNuvem.Data;

namespace Tecnu.Core.Record
{
    public class TeacherRecord : RecordBase
    {
        public virtual string Name { get; set; }

        public virtual DateTime DateBirth { get; set; }

        public virtual long Cpf { get; set; }
        public virtual long Telephone { get; set; }
    }
    public class TeacherMap : EntityMap<TeacherRecord>
    {
        public TeacherMap()
        {
            Table("PROFESSOR");
            Id(k => k.Id).Column("ID").GeneratedBy.Increment();
            Map(m => m.Name).Column("NNPROFESSOR").Length(500).Not.Nullable();
            Map(m => m.Cpf).Column("NUCPF").Not.Nullable();
            Map(m => m.DateBirth).Column("DTNASCIMENTO").Not.Nullable();
            Map(m => m.Telephone).Column("NUTELEFONE").Not.Nullable();
            Map(m => m.When).Column("DTCADASTRO").Not.Nullable();
            Map(m => m.Update).Column("DTATUALIZACAO").Not.Nullable();
        }
    }
}