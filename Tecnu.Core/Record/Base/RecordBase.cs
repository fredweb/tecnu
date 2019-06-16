using System;

namespace Tecnu.Core.Record.Base
{
    public class RecordBase
    {
        public virtual long Id { get; set; }
        public virtual DateTime When { get; set; }
        public virtual DateTime Update { get; set; }
    }
}