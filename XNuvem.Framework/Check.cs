using System;

namespace XNuvem
{
    internal static class Check
    {
        public static void IsNotNull(object value, string paramName)
        {
            if (value == null) throw new ArgumentNullException(paramName);
        }

        public static void IsNotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentException(paramName);
        }
    }
}