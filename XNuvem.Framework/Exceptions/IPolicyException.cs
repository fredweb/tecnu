using System;

namespace XNuvem.Exceptions
{
    public interface IPolicyException
    {
        bool HandleException(Exception ex);
    }
}