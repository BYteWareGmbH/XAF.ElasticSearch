namespace BYteWare.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Formatter Class for TimeSpans.
    /// </summary>
    public class TimeSpanFormatter : IFormatProvider, ICustomFormatter
    {
        private readonly Regex _formatParser;

        /// <summary>
        /// Initalizes a new instance of the <see cref="TimeSpanFormatter"/> class.
        /// </summary>
        public TimeSpanFormatter()
        {
            _formatParser = new Regex("d{1,2}|h{1,2}|H{1,5}|m{1,2}|s{1,2}|f{1,7}", RegexOptions.Compiled);
        }

        /// <summary>
        /// Returns a formatter for the formatType if fitting.
        /// </summary>
        /// <param name="formatType">The Formatter Type.</param>
        /// <returns>This if formatType is fitting, otherwise null.</returns>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Formats the arg with the format string and the formatProvider.
        /// </summary>
        /// <param name="format">format string.</param>
        /// <param name="arg">object to format.</param>
        /// <param name="formatProvider">format provider.</param>
        /// <returns>The formatted string.</returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is TimeSpan && _formatParser.IsMatch(format))
            {
                var timeSpan = (TimeSpan)arg;
                return (timeSpan < TimeSpan.Zero ? "-" : string.Empty) + _formatParser.Replace(format, GetMatchEvaluator(timeSpan));
            }
            else
            {
                try
                {
                    return HandleOtherFormats(format, arg);
                }
                catch (FormatException e)
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture, "The format of '{0}' is invalid.", format), e);
                }
            }
        }

        private static string HandleOtherFormats(string format, object arg)
        {
            var farg = arg as IFormattable;
            if (farg != null)
            {
                return farg.ToString(format, CultureInfo.CurrentCulture);
            }
            else
            {
                if (arg != null)
                {
                    return arg.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private static MatchEvaluator GetMatchEvaluator(TimeSpan timeSpan)
        {
            return m => EvaluateMatch(m, timeSpan);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Just a switch statement")]
        private static string EvaluateMatch(Match match, TimeSpan timeSpan)
        {
            switch (match.Value)
            {
                case "dd":
                    return timeSpan.Days.ToString("00;00", CultureInfo.CurrentCulture);
                case "d":
                    return timeSpan.Days.ToString("0;0", CultureInfo.CurrentCulture);
                case "H":
                    return ((int)timeSpan.TotalHours).ToString("0;0", CultureInfo.CurrentCulture);
                case "HH":
                    return ((int)timeSpan.TotalHours).ToString("00;00", CultureInfo.CurrentCulture);
                case "HHH":
                    return ((int)timeSpan.TotalHours).ToString("000;000", CultureInfo.CurrentCulture);
                case "HHHH":
                    return ((int)timeSpan.TotalHours).ToString("0000;0000", CultureInfo.CurrentCulture);
                case "HHHHH":
                    return ((int)timeSpan.TotalHours).ToString("00000;00000", CultureInfo.CurrentCulture);
                case "hh":
                    return timeSpan.Hours.ToString("00;00", CultureInfo.CurrentCulture);
                case "h":
                    return timeSpan.Hours.ToString("0;0", CultureInfo.CurrentCulture);
                case "mm":
                    return timeSpan.Minutes.ToString("00;00", CultureInfo.CurrentCulture);
                case "m":
                    return timeSpan.Minutes.ToString("0;0", CultureInfo.CurrentCulture);
                case "ss":
                    return timeSpan.Seconds.ToString("00;00", CultureInfo.CurrentCulture);
                case "s":
                    return timeSpan.Seconds.ToString("0;0", CultureInfo.CurrentCulture);
                case "fffffff":
                    return (timeSpan.Milliseconds * 10000).ToString("0000000;0000000", CultureInfo.CurrentCulture);
                case "ffffff":
                    return (timeSpan.Milliseconds * 1000).ToString("000000;000000", CultureInfo.CurrentCulture);
                case "fffff":
                    return (timeSpan.Milliseconds * 100).ToString("00000;00000", CultureInfo.CurrentCulture);
                case "ffff":
                    return (timeSpan.Milliseconds * 10).ToString("0000;0000", CultureInfo.CurrentCulture);
                case "fff":
                    return timeSpan.Milliseconds.ToString("000;000", CultureInfo.CurrentCulture);
                case "ff":
                    return (timeSpan.Milliseconds / 10).ToString("00;00", CultureInfo.CurrentCulture);
                case "f":
                    return (timeSpan.Milliseconds / 100).ToString("0;0", CultureInfo.CurrentCulture);
                default:
                    return match.Value;
            }
        }
    }
}
