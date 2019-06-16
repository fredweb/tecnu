/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 *
 * 
/****************************************************************************************/


using System;
using System.Runtime.Serialization;
using XNuvem.Localization;

namespace XNuvem
{
    [Serializable]
    public class XNuvemCoreException : Exception
    {
        public XNuvemCoreException(string message)
            : base(message)
        {
        }

        public XNuvemCoreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public XNuvemCoreException(LocalizedString message)
            : base(message.Text)
        {
            LocalizedMessage = message;
        }

        public XNuvemCoreException(LocalizedString message, Exception innerException)
            : base(message.Text, innerException)
        {
            LocalizedMessage = message;
        }

        protected XNuvemCoreException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public LocalizedString LocalizedMessage { get; }
    }
}