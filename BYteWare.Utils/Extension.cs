namespace BYteWare.Utils.Extension
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Collection of various useful extension methods.
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Regular Expression to test E-Mail Addresses.
        /// </summary>
        public const string EMailRegEx = @"^[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?$";

        /// <summary>
        /// Creates an empty List with elements of the Type of obj.
        /// </summary>
        /// <param name="obj">Object where the Type T is deduced from.</param>
        /// <typeparam name="T">Type for the elements of the list.</typeparam>
        /// <returns>Empty List of Type T.</returns>
#pragma warning disable IDE0060 // Nicht verwendete Parameter entfernen
        public static List<T> CreateEmptyList<T>(T obj)
#pragma warning restore IDE0060 // Nicht verwendete Parameter entfernen
        {
            return new List<T>();
        }

        /// <summary>
        /// Creates a List with the given elements.
        /// </summary>
        /// <param name="elements">The elements to add to the list.</param>
        /// <typeparam name="T">Type for the elements of the list.</typeparam>
        /// <returns>List with elements.</returns>
        public static List<T> CreateList<T>(params T[] elements)
        {
            return new List<T>(elements);
        }

        /// <summary>
        /// Returns null if the String str is null or empty.
        /// </summary>
        /// <param name="str">String to be tested.</param>
        /// <returns>Null if the string is null or empty, str otherwise.</returns>
        public static string AsNullIfEmpty(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return str;
        }

        /// <summary>
        /// Returns null if the Enumeration items is null or empty.
        /// </summary>
        /// <typeparam name="T">Type for the elements of the list.</typeparam>
        /// <param name="items">Enumeration to be tested.</param>
        /// <returns>Null if the Enumeration is null or empty, items otherwise.</returns>
        public static IEnumerable<T> AsNullIfEmpty<T>(this IEnumerable<T> items)
        {
            if (items == null || !items.Any())
            {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
                return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
            }
            return items;
        }

        /// <summary>
        /// Returns null if the String str is null or empty or contains only whitespace characters.
        /// </summary>
        /// <param name="str">String to be tested.</param>
        /// <returns>Null if the string is null or empty or contains only whitespace characters, str otherwise.</returns>
        public static string AsNullIfWhiteSpace(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            return str;
        }

        /// <summary>
        /// Returns true if the Collection is null or empty.
        /// </summary>
        /// <typeparam name="T">Type for the elements of the Collection.</typeparam>
        /// <param name="input">Collection to be tested.</param>
        /// <returns>True if the Collection is null or empty, false otherwise.</returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> input)
        {
            return (input == null) || (input.Count == 0);
        }

        /// <summary>
        /// Returns true if the Enumeration is null or empty.
        /// </summary>
        /// <typeparam name="T">Type for the elements of the Enumeration.</typeparam>
        /// <param name="source">Enumeration to be tested.</param>
        /// <returns>True if the Enumeration is null or empty, false otherwise.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return (source == null) || (!source.Any());
        }

        /// <summary>
        /// Returns true if the Enumeration has items.
        /// </summary>
        /// <typeparam name="T">Type for the elements of the Enumeration.</typeparam>
        /// <param name="source">Enumeration to be tested.</param>
        /// <returns>True if the Enumeration is not null or empty, false otherwise.</returns>
        public static bool HasValues<T>(this IEnumerable<T> source)
        {
            return !IsNullOrEmpty(source);
        }

        /// <summary>
        /// Returns the substring without throwing exceptions.
        /// </summary>
        /// <param name="text">The text to get the substring from.</param>
        /// <param name="start">starting character position.</param>
        /// <param name="length">maximum number of characters to copy.</param>
        /// <returns>The Substring from text starting at start with maximum length characters.</returns>
        public static string SafeSubstring(this string text, int start, int length)
        {
            if (text is null)
            {
                return null;
            }
            if (text.Length <= start)
            {
                return string.Empty;
            }
            if (text.Length - start <= length)
            {
                return text.Substring(start);
            }
            return text.Substring(start, length);
        }

        /// <summary>
        /// Get string value between [first] after and [last] until.
        /// </summary>
        /// <param name="value">String to be searched for.</param>
        /// <param name="after">Search for after.</param>
        /// <param name="until">End on until.</param>
        /// <returns>returns the String between the first a and the last b.</returns>
        public static string Between(this string value, string after, string until)
        {
            if (value == null)
            {
                return value;
            }
            after = after ?? string.Empty;
            until = until ?? string.Empty;
            var posA = value.IndexOf(after, StringComparison.OrdinalIgnoreCase);
            var posB = value.LastIndexOf(until, StringComparison.OrdinalIgnoreCase);
            if (posA == -1)
            {
                return string.Empty;
            }
            if (posB == -1)
            {
                return string.Empty;
            }
            var adjustedPosA = posA + after.Length;
            if (adjustedPosA >= posB)
            {
                return string.Empty;
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value before [first] until.
        /// </summary>
        /// <param name="value">String to be searched.</param>
        /// <param name="until">End at this string.</param>
        /// <returns>String value before first until.</returns>
        public static string Before(this string value, string until)
        {
            if (value == null)
            {
                return value;
            }
            until = until ?? string.Empty;
            var posA = value.IndexOf(until, StringComparison.OrdinalIgnoreCase);
            if (posA == -1)
            {
                return string.Empty;
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] aft.
        /// </summary>
        /// <param name="value">String to be searched.</param>
        /// <param name="aft">Begin at this string.</param>
        /// <returns>String value after last aft.</returns>
        public static string After(this string value, string aft)
        {
            if (value == null)
            {
                return value;
            }
            aft = aft ?? string.Empty;
            var posA = value.LastIndexOf(aft, StringComparison.OrdinalIgnoreCase);
            if (posA == -1)
            {
                return string.Empty;
            }
            var adjustedPosA = posA + aft.Length;
            if (adjustedPosA >= value.Length)
            {
                return string.Empty;
            }
            return value.Substring(adjustedPosA);
        }

        /// <summary>
        /// Formats a string with one literal placeholder.
        /// </summary>
        /// <param name="text">The extension text.</param>
        /// <param name="arg0">Argument 0.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatWith(this string text, object arg0)
        {
            return string.Format(CultureInfo.CurrentCulture, text, arg0);
        }

        /// <summary>
        /// Formats a string with two literal placeholder.
        /// </summary>
        /// <param name="text">The extension text.</param>
        /// <param name="arg0">Argument 0.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatWith(this string text, object arg0, object arg1)
        {
            return string.Format(CultureInfo.CurrentCulture, text, arg0, arg1);
        }

        /// <summary>
        /// Formats a string with tree literal placeholder.
        /// </summary>
        /// <param name="text">The extension text.</param>
        /// <param name="arg0">Argument 0.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatWith(this string text, object arg0, object arg1, object arg2)
        {
            return string.Format(CultureInfo.CurrentCulture, text, arg0, arg1, arg2);
        }

        /// <summary>
        /// Formats a string with a list of literal placeholder.
        /// </summary>
        /// <param name="text">The extension text.</param>
        /// <param name="args">The argument list.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatWith(this string text, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, text, args);
        }

        /// <summary>
        /// Formats a string with a list of literal placeholder.
        /// </summary>
        /// <param name="text">The extension text.</param>
        /// <param name="provider">The format provider.</param>
        /// <param name="args">The argument list.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatWith(this string text, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, text, args);
        }

        /// <summary>
        /// Parses a string into an Enum.
        /// </summary>
        /// <typeparam name="T">The type of the Enum.</typeparam>
        /// <param name="value">String value to parse.</param>
        /// <returns>The Enum corresponding to the stringExtensions.</returns>
        public static T ToEnum<T>(this string value)
        {
            return ToEnum<T>(value, false);
        }

        /// <summary>
        /// Parses a string into an Enum.
        /// </summary>
        /// <typeparam name="T">The type of the Enum.</typeparam>
        /// <param name="value">String value to parse.</param>
        /// <param name="ignorecase">Ignore the case of the string being parsed.</param>
        /// <returns>The Enum corresponding to the stringExtensions.</returns>
        public static T ToEnum<T>(this string value, bool ignorecase)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            value = value.Trim();

            if (value.Length == 0)
            {
                throw new ArgumentException("Must specify valid information for parsing in the string.", nameof(value));
            }

            var t = typeof(T);
            if (!t.IsEnum)
            {
#pragma warning disable CC0002 // Invalid argument name
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one
                throw new ArgumentException("Type provided must be an Enum.", "T");
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one
#pragma warning restore CC0002 // Invalid argument name
            }

            return (T)Enum.Parse(t, value, ignorecase);
        }

        /// <summary>
        /// Toes the integer.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultvalue">The default value.</param>
        /// <returns>Integer from String value.</returns>
        public static int ToInteger(this string value, int defaultvalue)
        {
            return (int)ToDouble(value, defaultvalue);
        }

        /// <summary>
        /// Toes the integer.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Integer from String value.</returns>
        public static int ToInteger(this string value)
        {
            return ToInteger(value, 0);
        }

        /// <summary>
        /// Toes the double.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultvalue">The default value.</param>
        /// <returns>Double from String value.</returns>
        public static double ToDouble(this string value, double defaultvalue)
        {
            if (double.TryParse(value, out double result))
            {
                return result;
            }
            else
            {
                return defaultvalue;
            }
        }

        /// <summary>
        /// Toes the double.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Double from String value.</returns>
        public static double ToDouble(this string value)
        {
            return ToDouble(value, 0);
        }

        /// <summary>
        /// Toes the date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultvalue">The default value.</param>
        /// <returns>DateTime from String value.</returns>
        public static DateTime? ToDateTime(this string value, DateTime? defaultvalue)
        {
            if (DateTime.TryParse(value, out DateTime result))
            {
                return result;
            }
            else
            {
                return defaultvalue;
            }
        }

        /// <summary>
        /// Toes the date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>DateTime from String value.</returns>
        public static DateTime? ToDateTime(this string value)
        {
            return ToDateTime(value, null);
        }

        /// <summary>
        /// Converts a string value to bool value, supports "T" and "F" conversions.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>A bool based on the string value.</returns>
        public static bool? ToBoolean(this string value)
        {
            if (string.Compare("T", value, true, CultureInfo.CurrentCulture) == 0)
            {
                return true;
            }
            if (string.Compare("F", value, true, CultureInfo.CurrentCulture) == 0)
            {
                return false;
            }
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts string to a Name-Format where each first letter is Uppercase.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>String With Each First Letter Uppercase.</returns>
        public static string ToUpperLowerNameVariant(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var valuearray = value.ToLower(CultureInfo.CurrentCulture).ToCharArray();
            var nextupper = true;
            for (int i = 0; i < (valuearray.Count() - 1); i++)
            {
                if (nextupper)
                {
                    valuearray[i] = char.Parse(valuearray[i].ToString().ToUpper(CultureInfo.CurrentCulture));
                    nextupper = false;
                }
                else
                {
                    switch (valuearray[i])
                    {
                        case ' ':
                        case '-':
                        case '.':
                        case ':':
                        case '\n':
                            nextupper = true;
                            break;
                        default:
                            nextupper = false;
                            break;
                    }
                }
            }
            return new string(valuearray);
        }

        /// <summary>
        /// Determines whether it is a valid URL.
        /// </summary>
        /// <param name="text">The String to test.</param>
        /// <returns>
        ///     <c>true</c> if [is valid URL] [the specified text]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidUrl(this string text)
        {
            return Regex.IsMatch(text, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Compiled);
        }

        /// <summary>
        /// Determines whether it is a valid email address.
        /// </summary>
        /// <param name="email">The String to test.</param>
        /// <returns>
        ///     <c>true</c> if [is valid email address] [the specified s]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidEmailAddress(this string email)
        {
            return Regex.IsMatch(email, EMailRegEx, RegexOptions.Compiled);
        }

        /// <summary>
        /// Truncates the string to a specified length and replace the truncated to a ...
        /// </summary>
        /// <param name="text">The String to truncate.</param>
        /// <param name="maxLength">total length of characters to maintain before the truncate happens.</param>
        /// <returns>truncated string.</returns>
        public static string Truncate(this string text, int maxLength)
        {
            // replaces the truncated string to a ...
            const string suffix = "...";
            var truncatedString = text;

            if (maxLength <= 0)
            {
                return truncatedString;
            }

            var strLength = maxLength - suffix.Length;

            if (strLength <= 0)
            {
                return truncatedString;
            }

            if (text == null || text.Length <= maxLength)
            {
                return truncatedString;
            }

            truncatedString = text.Substring(0, strLength);
            truncatedString = truncatedString.TrimEnd();
            truncatedString += suffix;
            return truncatedString;
        }

#pragma warning disable S125 // Sections of code should not be commented out
        /*/// <summary>
                /// Converts to a HTML-encoded string
                /// </summary>
                /// <param name="data">The data.</param>
                /// <returns></returns>
                public static string HtmlEncode(this string data)
                {
                    return System.Web.HttpUtility.HtmlEncode(data);
                }

                /// <summary>
                /// Converts the HTML-encoded string into a decoded string
                /// </summary>
                public static string HtmlDecode(this string data)
                {
                    return System.Web.HttpUtility.HtmlDecode(data);
                }

                /// <summary>
                /// Parses a query string into a System.Collections.Specialized.NameValueCollection
                /// using System.Text.Encoding.UTF8 encoding.
                /// </summary>
                public static System.Collections.Specialized.NameValueCollection ParseQueryString(this string query)
                {
                    return System.Web.HttpUtility.ParseQueryString(query);
                }

                /// <summary>
                /// Encode an Url string
                /// </summary>
                public static string UrlEncode(this string url)
                {
                    return System.Web.HttpUtility.UrlEncode(url);
                }

                /// <summary>
                /// Converts a string that has been encoded for transmission in a URL into a
                /// decoded string.
                /// </summary>
                public static string UrlDecode(this string url)
                {
                    return System.Web.HttpUtility.UrlDecode(url);
                }

                /// <summary>
                /// Encodes the path portion of a URL string for reliable HTTP transmission from
                /// the Web server to a client.
                /// </summary>
                public static string UrlPathEncode(this string url)
                {
                    return System.Web.HttpUtility.UrlPathEncode(url);
                }*/
#pragma warning restore S125 // Sections of code should not be commented out

        /// <summary>
        /// Determines whether [is not null or empty] [the specified input].
        /// </summary>
        /// <param name="input">The String to test.</param>
        /// <returns>
        ///     <c>true</c> if [is not null or empty] [the specified input]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNullOrEmpty(this string input)
        {
            return !string.IsNullOrEmpty(input);
        }

        /// <summary>
        /// Determines whether [is not null or empty or only contains space characters] [the specified input].
        /// </summary>
        /// <param name="input">The string to test.</param>
        /// <returns>
        ///     <c>true</c> if [is not null or empty] [the specified input]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNullOrWhiteSpace(this string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        /// <summary>
        /// Returns true if enum matches any of the given values.
        /// </summary>
        /// <param name="value">Value to match.</param>
        /// <param name="values">Values to match against.</param>
        /// <returns>Return true if matched.</returns>
        public static bool In(this Enum value, params Enum[] values)
        {
            return values.Any(v => v == value);
        }

        /// <summary>
        /// Checks string object's value to array of string values.
        /// </summary>
        /// <param name="value">The String to test.</param>
        /// <param name="values">Array of string values to compare.</param>
        /// <returns>Return true if any string value matches.</returns>
        public static bool In(this string value, params string[] values)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            foreach (string otherValue in values)
            {
                if (string.Compare(value, otherValue, StringComparison.CurrentCulture) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns characters from right of specified length.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <param name="length">Max number of characters to return.</param>
        /// <returns>Returns string from right.</returns>
        public static string Right(this string value, int length)
        {
            return value != null && value.Length > length ? value.Substring(value.Length - length) : value;
        }

        /// <summary>
        /// Returns characters from left of specified length.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <param name="length">Max number of characters to return.</param>
        /// <returns>Returns string from left.</returns>
        public static string Left(this string value, int length)
        {
            return value != null && value.Length > length ? value.Substring(0, length) : value;
        }

        /// <summary>
        /// Convert hex String to bytes representation.
        /// </summary>
        /// <param name="value">Hex string to convert into bytes.</param>
        /// <returns>Bytes of hex string.</returns>
        public static byte[] HexToBytes(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (value.Length % 2 != 0)
            {
                throw new ArgumentException(Current($"Length of {nameof(value)} cannot be an odd number: {value}"));
            }

            var retVal = new byte[value.Length / 2];
            for (int i = 0; i < value.Length; i += 2)
            {
                retVal[i / 2] = byte.Parse(value.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return retVal;
        }

        /// <summary>
        /// Adds the String append to each Word in String text, that doesn't already contain append.
        /// </summary>
        /// <param name="text">The String where each word is appended with c.</param>
        /// <param name="append">The string to append.</param>
        /// <returns>The string with each word in text appended with append.</returns>
        public static string AddStringToWordEnd(this string text, string append)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            var sl = new List<string>();
            foreach (var wrd in text.Split(' '))
            {
                var str = wrd.Trim();
                if (str.IsNotNullOrEmpty())
                {
                    if (!str.Contains(append))
                    {
                        str += append;
                    }
                    sl.Add(str);
                }
            }
            return string.Join(" ", sl.ToArray());
        }

        /// <summary>
        /// Checks if each word in the String text, contains str.
        /// </summary>
        /// <param name="text">The String to test.</param>
        /// <param name="str">The String to be searched for.</param>
        /// <returns>
        ///     <c>true</c> if every word in text contains str otherwise, <c>false</c>.
        /// </returns>
        public static bool EveryWordContains(this string text, string str)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            return text.Split(' ').Where(t => t.IsNotNullOrWhiteSpace()).All(t => t.Contains(str));
        }

        /// <summary>
        /// Enumerates obj and all its children, which are recursively fetched through the function children.
        /// </summary>
        /// <typeparam name="T">Type for the elements of the Enumeration.</typeparam>
        /// <param name="obj">The Object to start with.</param>
        /// <param name="children">The function which delivers the children.</param>
        /// <returns>Enumeration of obj and all its children, recursively.</returns>
        public static IEnumerable<T> DescendantsAndSelf<T>(this T obj, Func<T, IEnumerable<T>> children)
        {
            if (obj != null)
            {
                yield return obj;

                var stack = new Stack<T>();
                stack.Push(obj);
                var visited = new HashSet<T>
                {
                    obj,
                };

                while (stack.Count > 0)
                {
                    var element = stack.Pop();
                    var ch = children?.Invoke(element);
                    if (ch != null)
                    {
                        foreach (var item in ch)
                        {
                            if (item != null && visited.Add(item))
                            {
                                yield return item;
                                stack.Push(item);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests if name fits the mask expression.
        /// </summary>
        /// <param name="name">The String to be tested.</param>
        /// <param name="expression">The expression with optional wildcard characters '*', '?', '>', '>', '"'. </param>
        /// <returns>
        ///     <c>true</c> if name fits the mask expression otherwise, <c>false</c>.
        /// </returns>
        public static bool FitsMask(this string name, string expression)
        {
#pragma warning disable S1854 // Unused assignments should be removed
#pragma warning disable S907 // "goto" statement should not be used
#pragma warning disable CC0120 // Your Switch maybe include default clause
#pragma warning disable IDE0059 // Unnötige Zuweisung eines Werts.
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            expression = expression.ToLowerInvariant();
            name = name.ToLowerInvariant();
            int num9;
            var ch = '\0';
            var sourceArray = new int[16];
            var numArray2 = new int[16];
            var flag = false;
            if (((name == null) || (name.Length == 0)) || ((expression == null) || (expression.Length == 0)))
            {
                return false;
            }
            if (expression.Equals("*") || expression.Equals("*.*"))
            {
                return true;
            }
            if ((expression[0] == '*') && (expression.IndexOf('*', 1) == -1))
            {
                var length = expression.Length - 1;
                if ((name.Length >= length) && (string.Compare(expression, 1, name, name.Length - length, length, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    return true;
                }
            }
            sourceArray[0] = 0;
            var num7 = 1;
            var num = 0;
            var num8 = expression.Length * 2;
            while (!flag)
            {
                int num3;
                if (num < name.Length)
                {
                    ch = name[num];
                    num3 = 1;
                    num++;
                }
                else
                {
                    flag = true;
                    if (sourceArray[num7 - 1] == num8)
                    {
                        break;
                    }
                }
                var index = 0;
                var num5 = 0;
                var num6 = 0;
                while (index < num7)
                {
                    var num2 = (sourceArray[index++] + 1) / 2;
                    num3 = 0;
                Label_00F2:
                    if (num2 != expression.Length)
                    {
                        num2 += num3;
                        num9 = num2 * 2;
                        if (num2 == expression.Length)
                        {
                            numArray2[num5++] = num8;
                        }
                        else
                        {
                            var ch2 = expression[num2];
                            num3 = 1;
                            if (num5 >= 14)
                            {
                                var num11 = numArray2.Length * 2;
                                var destinationArray = new int[num11];
                                Array.Copy(numArray2, destinationArray, numArray2.Length);
                                numArray2 = destinationArray;
                                destinationArray = new int[num11];
                                Array.Copy(sourceArray, destinationArray, sourceArray.Length);
                                sourceArray = destinationArray;
                            }
                            if (ch2 == '*')
                            {
                                numArray2[num5++] = num9;
                                numArray2[num5++] = num9 + 1;
                                goto Label_00F2;
                            }
                            if (ch2 == '>')
                            {
                                var flag2 = false;
                                if (!flag && (ch == '.'))
                                {
                                    var num13 = name.Length;
                                    for (int i = num; i < num13; i++)
                                    {
                                        var ch3 = name[i];
                                        num3 = 1;
                                        if (ch3 == '.')
                                        {
                                            flag2 = true;
                                            break;
                                        }
                                    }
                                }
                                if ((flag || (ch != '.')) || flag2)
                                {
                                    numArray2[num5++] = num9;
                                    numArray2[num5++] = num9 + 1;
                                }
                                else
                                {
                                    numArray2[num5++] = num9 + 1;
                                }
                                goto Label_00F2;
                            }
                            num9 += num3 * 2;
                            switch (ch2)
                            {
                                case '<':
                                    if (flag || (ch == '.'))
                                    {
                                        goto Label_00F2;
                                    }
                                    numArray2[num5++] = num9;
                                    goto Label_028D;

                                case '"':
                                    if (flag)
                                    {
                                        goto Label_00F2;
                                    }
                                    if (ch == '.')
                                    {
                                        numArray2[num5++] = num9;
                                        goto Label_028D;
                                    }
                                    break;
                            }
                            if (!flag)
                            {
                                if (ch2 == '?')
                                {
                                    numArray2[num5++] = num9;
                                }
                                else if (ch2 == ch)
                                {
                                    numArray2[num5++] = num9;
                                }
                            }
                        }
                    }
                Label_028D:
                    if ((index < num7) && (num6 < num5))
                    {
                        while (num6 < num5)
                        {
                            var num14 = sourceArray.Length;
                            while ((index < num14) && (sourceArray[index] < numArray2[num6]))
                            {
                                index++;
                            }
                            num6++;
                        }
                    }
                }
                if (num5 == 0)
                {
                    return false;
                }
                var numArray4 = sourceArray;
                sourceArray = numArray2;
                numArray2 = numArray4;
                num7 = num5;
            }
            num9 = sourceArray[num7 - 1];
            return num9 == num8;
#pragma warning restore IDE0059 // Unnötige Zuweisung eines Werts.
#pragma warning restore CC0120 // Your Switch maybe include default clause
#pragma warning restore S907 // "goto" statement should not be used
#pragma warning restore S1854 // Unused assignments should be removed
        }

        private class ChunkedEnumerable<T> : IEnumerable<T>
        {
            private class ChildEnumerator : IEnumerator<T>
            {
                private readonly ChunkedEnumerable<T> parent;
                private bool _Disposed;
                private T current;
                private bool done;
                private int position;

                public ChildEnumerator(ChunkedEnumerable<T> parent)
                {
                    this.parent = parent;
                    position = -1;
                    parent.wrapper.AddRef();
                }

                ~ChildEnumerator()
                {
                    Dispose(false);
                }

                public T Current
                {
                    get
                    {
                        if (position == -1 || done)
                        {
                            throw new InvalidOperationException();
                        }
                        return current;
                    }
                }

                object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        return Current;
                    }
                }

                public void Dispose()
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }

                public bool MoveNext()
                {
                    position++;

                    if (position + 1 > parent.chunkSize)
                    {
                        done = true;
                    }

                    if (!done)
                    {
                        done = !parent.wrapper.Get(position + parent.start, out current);
                    }

                    return !done;
                }

                public void Reset()
                {
                    // per http://msdn.microsoft.com/en-us/library/system.collections.ienumerator.reset.aspx
                    throw new NotSupportedException();
                }

                protected virtual void Dispose(bool disposing)
                {
                    if (!_Disposed && disposing)
                    {
                        done = true;
                        parent.wrapper.RemoveRef();
                    }

                    _Disposed = true;
                }
            }

            private readonly EnumeratorWrapper<T> wrapper;
            private readonly int chunkSize;
            private readonly int start;

            public ChunkedEnumerable(EnumeratorWrapper<T> wrapper, int chunkSize, int start)
            {
                this.wrapper = wrapper;
                this.chunkSize = chunkSize;
                this.start = start;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new ChildEnumerator(this);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class EnumeratorWrapper<T>
        {
            public EnumeratorWrapper(IEnumerable<T> source)
            {
                SourceEnumerable = source;
            }

            private IEnumerable<T> SourceEnumerable
            {
                get;
                set;
            }

            private Enumeration currentEnumeration;

            private class Enumeration
            {
                public IEnumerator<T> Source
                {
                    get;
                    set;
                }
                public int Position
                {
                    get;
                    set;
                }
                public bool AtEnd
                {
                    get;
                    set;
                }
            }

            public bool Get(int pos, out T item)
            {
                if (currentEnumeration != null && currentEnumeration.Position > pos)
                {
                    currentEnumeration.Source.Dispose();
                    currentEnumeration = null;
                }

                if (currentEnumeration == null)
                {
                    currentEnumeration = new Enumeration
                    {
                        Position = -1,
                        Source = SourceEnumerable.GetEnumerator(),
                        AtEnd = false,
                    };
                }

                item = default;
                if (currentEnumeration.AtEnd)
                {
                    return false;
                }

                while (currentEnumeration.Position < pos)
                {
                    currentEnumeration.AtEnd = !currentEnumeration.Source.MoveNext();
                    currentEnumeration.Position++;

                    if (currentEnumeration.AtEnd)
                    {
                        return false;
                    }
                }

                item = currentEnumeration.Source.Current;

                return true;
            }

#pragma warning disable CRRSP12 // A misspelled word has been found
            private int refs;
#pragma warning restore CRRSP12 // A misspelled word has been found

            // needed for dispose semantics
            public void AddRef()
            {
                refs++;
            }

            public void RemoveRef()
            {
                refs--;
                if (refs == 0 && currentEnumeration != null)
                {
                    var copy = currentEnumeration;
                    currentEnumeration = null;
                    copy.Source.Dispose();
                }
            }
        }

#pragma warning disable CRRSP04 // A misspelled word has been found
        /// <summary>
        /// Splits the Enumeration source into chunks with maximum chunksize elements.
        /// </summary>
        /// <typeparam name="T">Type for the elements of the Enumeration.</typeparam>
        /// <param name="source">The source Enumeration.</param>
        /// <param name="chunksize">The maximum number of elements of a chunk.</param>
        /// <returns>An Enumeration of Enumerations of maximum chunksize elements.</returns>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
#pragma warning restore CRRSP04 // A misspelled word has been found
        {
            if (chunksize < 1)
            {
                throw new InvalidOperationException();
            }

            var wrapper = new EnumeratorWrapper<T>(source);

            var currentPos = 0;
            try
            {
                wrapper.AddRef();
                while (wrapper.Get(currentPos, out T ignore))
                {
                    yield return new ChunkedEnumerable<T>(wrapper, chunksize, currentPos);
                    currentPos += chunksize;
                }
            }
            finally
            {
                wrapper.RemoveRef();
            }
        }

        /// <summary>
        /// Returns a nullable int from the DataReader with number column.
        /// </summary>
        /// <param name="reader">The DataReader.</param>
        /// <param name="column">The number of the column to return.</param>
        /// <returns>Null if the columns value is null otherwise the integer value.</returns>
        public static int? GetNullableInt32(this IDataRecord reader, int column)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            return reader.IsDBNull(column) ? (int?)null : reader.GetInt32(column);
        }

        /// <summary>
        /// Delivers the minimum of the two comparable values val1 and val2.
        /// </summary>
        /// <typeparam name="T">Type for comparison.</typeparam>
        /// <param name="val1">The value val1 to compare.</param>
        /// <param name="val2">The value val2 to compare.</param>
        /// <returns>The smaller of the two values val1 and val2.</returns>
        public static T Min<T>(T val1, T val2) where T : IComparable
        {
            return val1.CompareTo(val2) <= 0 ? val1 : val2;
        }

        /// <summary>
        /// Delivers the maximum of the two comparable values val1 and val2.
        /// </summary>
        /// <typeparam name="T">Type for comparison.</typeparam>
        /// <param name="val1">The value val1 to compare.</param>
        /// <param name="val2">The value val2 to compare.</param>
        /// <returns>The bigger of the two values val1 and val2.</returns>
        public static T Max<T>(T val1, T val2) where T : IComparable
        {
            return val1.CompareTo(val2) >= 0 ? val1 : val2;
        }

        /// <summary>
        /// Returns a MemoryStream with the bytes of the String str in the given Encoding.
        /// </summary>
        /// <param name="str">The String where the bytes should be returned from.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>A MemoryStream with the bytes from str.</returns>
        public static MemoryStream ToStream(this string str, Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }
            return new MemoryStream(encoding.GetBytes(str ?? string.Empty));
        }

        /// <summary>
        /// Returns a MemoryStream with the bytes of the String str in UTF8 Encoding.
        /// </summary>
        /// <param name="str">The String where the bytes should be returned from.</param>
        /// <returns>A MemoryStream with the bytes from str.</returns>
        public static MemoryStream ToStream(this string str)
        {
            return str.ToStream(Encoding.UTF8);
        }

        /// <summary>
        /// Returns a byte array with the bytes of the String str.
        /// </summary>
        /// <param name="str">The String where the bytes should be returned from.</param>
        /// <returns>A byte array with the bytes from str.</returns>
        public static byte[] GetBytes(this string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Returns a string with the bytes of the byte array bytes.
        /// </summary>
        /// <param name="values">The byte array with the characters.</param>
        /// <returns>A string with the characters from bytes.</returns>
        public static string GetString(byte[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            var chars = new char[values.Length / sizeof(char)];
            Buffer.BlockCopy(values, 0, chars, 0, values.Length);
            return new string(chars);
        }

        /// <summary>
        /// Tests if the given Type is derived from Nullable.
        /// </summary>
        /// <param name="theType">The Type to test.</param>
        /// <returns>True if the Type is derived from Nullable otherwise false.</returns>
        public static bool IsNullableType(this Type theType)
        {
            if (theType == null)
            {
                throw new ArgumentNullException(nameof(theType));
            }
            if (theType.IsGenericType)
            {
                var genericTypeDefinition = theType.GetGenericTypeDefinition();
                if (genericTypeDefinition != null)
                {
                    return genericTypeDefinition == typeof(Nullable<>);
                }
            }
            return false;
        }

        /// <summary>
        /// Tests if the two given byte arrays contain the same sequence.
        /// </summary>
        /// <param name="b1">The byte array b1 to compare.</param>
        /// <param name="b2">The byte array b2 to compare.</param>
        /// <returns>True if the two byte arrays contain the same sequence of bytes.</returns>
        public static bool SequenceEqual(this byte[] b1, byte[] b2)
        {
            if (b1 == b2)
            {
                return true; // reference equality check
            }

            if (b1 == null || b2 == null || b1.Length != b2.Length)
            {
                return false;
            }

            return SafeNativeMethods.memcmp(b1, b2, new UIntPtr((uint)b1.Length)) == 0;
        }

        /// <summary>
        /// Format the given object in the current culture. This static method may be
        /// imported in C# by.
        /// <code>
        /// using static System.FormattableString;
        /// </code>.
        /// Within the scope
        /// of that import directive an interpolated string may be formatted in the
        /// current culture by writing, for example,.
        /// <code>
        /// Current($"{{ lat = {latitude}; lon = {longitude} }}")
        /// </code>
        /// </summary>
        /// <param name="formattable">The interpolated String.</param>
        /// <returns>The interpolated String with the parameters formatted in the Current culture.</returns>
        [CLSCompliant(false)]
        public static string Current(FormattableString formattable)
        {
            if (formattable == null)
            {
                throw new ArgumentNullException(nameof(formattable));
            }

            return formattable.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Determines if a type is numeric.  Nullable numeric types are considered numeric.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <remarks>
        /// Boolean is not considered numeric.
        /// </remarks>
        /// <returns>True if the type is numeric; False otherwise.</returns>
        public static bool IsNumericType(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Returns a read only Set.
        /// </summary>
        /// <typeparam name="T">The element Type.</typeparam>
        /// <param name="set">The set to return a read only set for.</param>
        /// <returns>A Read Only Set for set.</returns>
        public static ReadOnlySet<T> AsReadOnly<T>(this ISet<T> set)
        {
            return new ReadOnlySet<T>(set);
        }

        /// <summary>
        /// Returns a read only Collection.
        /// </summary>
        /// <typeparam name="T">The element Type.</typeparam>
        /// <param name="collection">The collection to return a read only collection for.</param>
        /// <returns>A Read Only collection for collection.</returns>
        public static RealReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection)
        {
            return new RealReadOnlyCollection<T>(collection);
        }

        /// <summary>
        /// Returns the default value for the Type tp.
        /// </summary>
        /// <param name="tp">The Type.</param>
        /// <returns>Default Value for Type tp.</returns>
        public static object GetDefaultValue(this Type tp)
        {
            if (tp == null)
            {
                throw new ArgumentNullException(nameof(tp));
            }
            if (tp.IsValueType)
            {
                return Activator.CreateInstance(tp);
            }

            return null;
        }

        /// <summary>
        /// Returns if the value for the Type tp is the default value.
        /// </summary>
        /// <param name="tp">The Type.</param>
        /// <param name="value">The Value.</param>
        /// <returns>True if value equals the default value of Type tp; otherwise False.</returns>
        public static bool IsDefaultValue(this Type tp, object value)
        {
            if (tp == null)
            {
                throw new ArgumentNullException(nameof(tp));
            }
            if (tp.IsValueType)
            {
                return Equals(value, Activator.CreateInstance(tp));
            }
            return value == null;
        }

        /// <summary>
        /// Returns if the value for the Type tp is the default value.
        /// </summary>
        /// <typeparam name="T">The Type.</typeparam>
        /// <param name="value">The Value.</param>
        /// <returns>True if value equals the default value of Type tp; otherwise False.</returns>
        public static bool IsDefaultValue<T>(this T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default);
        }

        /// <summary>
        /// Returns a two key level Dictionary.
        /// </summary>
        /// <typeparam name="TElement">Type for the elements of the enumeration.</typeparam>
        /// <typeparam name="TKey1">Type for the first key level.</typeparam>
        /// <typeparam name="TKey2">Type for the second key level.</typeparam>
        /// <typeparam name="TValue">Type for the elements.</typeparam>
        /// <param name="items">Source Enumeration.</param>
        /// <param name="key1">Selector Function for the first key level.</param>
        /// <param name="key2">Selector Function for the second key level.</param>
        /// <param name="value">Selector Function for the values of the Dictionary.</param>
        /// <returns>Dual Key Dictionary.</returns>
        public static DualKeyDictionary<TKey1, TKey2, TValue> ToDualKeyDictionary<TElement, TKey1, TKey2, TValue>(this IEnumerable<TElement> items, Func<TElement, TKey1> key1, Func<TElement, TKey2> key2, Func<TElement, TValue> value)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (key1 == null)
            {
                throw new ArgumentNullException(nameof(key1));
            }
            if (key2 == null)
            {
                throw new ArgumentNullException(nameof(key2));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var dict = new DualKeyDictionary<TKey1, TKey2, TValue>();
            foreach (TElement i in items)
            {
                dict.Add(key1(i), key2(i), value(i));
            }
            return dict;
        }

        /// <summary>
        /// Returns a three key level Dictionary.
        /// </summary>
        /// <typeparam name="TElement">Type for the elements of the enumeration.</typeparam>
        /// <typeparam name="TKey1">Type for the first key level.</typeparam>
        /// <typeparam name="TKey2">Type for the second key level.</typeparam>
        /// <typeparam name="TKey3">Type for the third key level.</typeparam>
        /// <typeparam name="TValue">Type for the elements.</typeparam>
        /// <param name="items">Source Enumeration.</param>
        /// <param name="key1">Selector Function for the first key level.</param>
        /// <param name="key2">Selector Function for the second key level.</param>
        /// <param name="key3">Selector Function for the third key level.</param>
        /// <param name="value">Selector Function for the values of the Dictionary.</param>
        /// <returns>Triple Key Dictionary.</returns>
        public static TripleKeyDictionary<TKey1, TKey2, TKey3, TValue> ToTripleKeyDictionary<TElement, TKey1, TKey2, TKey3, TValue>(this IEnumerable<TElement> items, Func<TElement, TKey1> key1, Func<TElement, TKey2> key2, Func<TElement, TKey3> key3, Func<TElement, TValue> value)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (key1 == null)
            {
                throw new ArgumentNullException(nameof(key1));
            }
            if (key2 == null)
            {
                throw new ArgumentNullException(nameof(key2));
            }
            if (key3 == null)
            {
                throw new ArgumentNullException(nameof(key3));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var dict = new TripleKeyDictionary<TKey1, TKey2, TKey3, TValue>();
            foreach (TElement i in items)
            {
                dict.Add(key1(i), key2(i), key3(i), value(i));
            }
            return dict;
        }
    }
}
