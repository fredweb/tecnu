/****************************************************************************************
 *
 * Autor: George Santos 
 * Copyright (c) 2016  
 * 
/****************************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using XNuvem.Security;

namespace XNuvem.Exceptions
{
    public static class ExceptionExtensions
    {
        public static bool IsFatal(this Exception ex)
        {
            return ex is XNuvemSecurityException ||
                   ex is StackOverflowException ||
                   ex is OutOfMemoryException ||
                   ex is AccessViolationException ||
                   ex is AppDomainUnloadedException ||
                   ex is ThreadAbortException ||
                   ex is SecurityException ||
                   ex is SEHException;
        }
    }
}