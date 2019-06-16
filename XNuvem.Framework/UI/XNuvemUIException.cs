using System;

namespace XNuvem.UI
{
    public class XNuvemUIException : XNuvemCoreException
    {
        public XNuvemUIException(string message)
            : base(message)
        {
        }

        public XNuvemUIException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}