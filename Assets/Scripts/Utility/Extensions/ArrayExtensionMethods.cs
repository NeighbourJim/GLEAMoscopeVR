using System;

namespace GLEAMoscopeVR.Utility.Extensions
{
    public static class ArrayExtensionMethods
    {
        public static bool IsNullOrEmpty(this Array array)
        {
            return array == null || array.Length == 0;
        }
    }
}