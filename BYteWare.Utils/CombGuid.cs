namespace BYteWare.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Class to generate Comb Guids.
    /// </summary>
    public static class CombGuid
    {
        private static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;
        private static int sequentialUuidCounter;

        /// <summary>
        /// Generate a new <see cref="Guid"/> using the comb algorithm.
        /// </summary>
        /// <returns>Guid Value with a incremental "end" to be consecutively indexed in SQL Server.</returns>
        public static Guid GenerateComb()
        {
            var guidArray = Guid.NewGuid().ToByteArray();

            var increment = Interlocked.Increment(ref sequentialUuidCounter);
            guidArray[7] = (byte)((increment << 4 & 0xf0) | (0x0f & guidArray[7]));
            var now = DateTime.UtcNow;

            // Get the days and milliseconds which will be used to build the byte string
            var days = new TimeSpan(now.Ticks - BaseDateTicks);
            var msecs = now.TimeOfDay;

            // Convert to a byte array
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333
            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }
    }
}
