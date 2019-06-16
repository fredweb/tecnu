using System.Collections.Generic;

namespace XNuvem.UI.DataTable
{
    public class DataTableResult
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public IEnumerable<object> data { get; set; }
        public string error { get; set; }
    }
}