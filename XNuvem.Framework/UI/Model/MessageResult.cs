using System.Collections.Generic;

namespace XNuvem.UI.Model
{
    public class MessageResult
    {
        public bool IsError { get; set; }

        public IEnumerable<string> Messages { get; set; }
    }
}