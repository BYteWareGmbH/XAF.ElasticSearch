namespace BYteWare.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Native Method Import
    /// </summary>
    [SuppressUnmanagedCodeSecurityAttribute]
    internal static class SafeNativeMethods
    {
        /// <summary>
        /// memcmp Method in msvcrt.dll
        /// </summary>
        /// <param name="b1">first byte array</param>
        /// <param name="b2">second byte array</param>
        /// <param name="count">Number of bytes to compare</param>
        /// <returns>0 if the two byte arrays are equal for count bytes</returns>
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int memcmp(byte[] b1, byte[] b2, UIntPtr count);
    }
}
