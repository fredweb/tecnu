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

namespace XNuvem.Security
{
    [Serializable]
    public class XNuvemSecurityException : XNuvemCoreException
    {
        public XNuvemSecurityException(LocalizedString message) : base(message)
        {
        }

        public XNuvemSecurityException(LocalizedString message, Exception innerException) : base(message,
            innerException)
        {
        }

        protected XNuvemSecurityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string PermissionName { get; set; }
        public User User { get; set; }
    }
}