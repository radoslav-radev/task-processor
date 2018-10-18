using System.Globalization;

namespace Radoslav
{
    public static partial class Helpers
    {
        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object
        /// in a specified array using the <see cref="CultureInfo.InvariantCulture"/> format provider.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args"> An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args using invariant format provider.</returns>
        /// <exception cref="System.ArgumentNullException">Parameter <paramref name="format"/> or <paramref name="args"/> is null.</exception>
        public static string FormatInvariant(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }
    }
}