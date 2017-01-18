namespace BYteWare.Utils
{
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Extension Methods for TimeSpans
    /// </summary>
    public static class TimeSpanExtensions
    {
        private static int[] weights = { 60 * 60 * 1000, 60 * 1000, 1000, 1 };

        /// <summary>
        /// Returns the TimeSpan value for string value
        /// </summary>
        /// <param name="value">The String to convert</param>
        /// <returns>TimeSpan from s</returns>
        public static TimeSpan ToTimeSpan(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            return TimeSpan.FromMilliseconds(value.Split('.', ':')
                .Zip(weights, (d, w) => Convert.ToInt64(d, CultureInfo.CurrentCulture) * w).Sum());
        }

        /// <summary>
        /// Returns the string to be used inside a format string for the given format
        /// </summary>
        /// <param name="format">Format String</param>
        /// <returns>The Format string for format</returns>
        public static string GetFormatString(this string format)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }
            var shouldModifyFormatString = format.IndexOf('{') == -1;
            return !shouldModifyFormatString ? format : ("{0" + ((!string.IsNullOrEmpty(format)) ? ":" + format : string.Empty) + "}");
        }

        /// <summary>
        /// Returns the TimeSpan ts formatted in the format string
        /// </summary>
        /// <param name="ts">The TimeStamp to format</param>
        /// <param name="format">The format string</param>
        /// <returns>ts formatted with format</returns>
        public static string ToFormattedString(this TimeSpan ts, string format)
        {
            return string.Format(new TimeSpanFormatter(), format.GetFormatString(), ts);
        }

        /// <summary>
        /// Returns the TimeSpan ts formatted with format "H:mm"
        /// </summary>
        /// <param name="ts">The TimeStamp to format</param>
        /// <returns>ts formatted</returns>
        public static string ToFormattedString(this TimeSpan ts)
        {
            return ts.ToFormattedString("H:mm");
        }
    }
}
