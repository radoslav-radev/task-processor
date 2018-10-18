using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Radoslav.Redis
{
    /// <summary>
    /// Class to convert values to and from Redis.
    /// </summary>
    public static class RedisConverter
    {
        /// <summary>
        /// Field to store an entity type in Redis hash.
        /// </summary>
        public const string EntityTypeKey = "Entity$Type";

        private const char ArraySeparatorAsChar = '|';
        private const string ArraySeparatorAsString = "|";

        private static readonly HashSet<Type> SupportedBaseTypes = new HashSet<Type>()
        {
            typeof(string),
            typeof(bool),
            typeof(int),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(Guid),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Type),
            typeof(Version)
        };

        /// <summary>
        /// Checks if a <see cref="Type"/> is supported by <see cref="RedisConverter"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <returns>True if type is supported by <see cref="RedisConverter"/>; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="type"/> is null.</exception>
        public static bool IsSupported(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                type = type.GetGenericArguments()[0];
            }

            return type.IsEnum || RedisConverter.SupportedBaseTypes.Contains(type);
        }

        /// <summary>
        /// Converts an <see cref="Boolean"/> value to a string that can be stored in Redis.
        /// </summary>
        /// <param name="value">The <see cref="Boolean"/> value to convert.</param>
        /// <returns>A string representation of the specified <see cref="Boolean"/> value that can be stored in Redis.</returns>
        public static string ToString(bool value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts an <see cref="Int32"/> value to a string that can be stored in Redis.
        /// </summary>
        /// <param name="value">The <see cref="Int32"/> value to convert.</param>
        /// <returns>A string representation of the specified <see cref="Int32"/> value that can be stored in Redis.</returns>
        public static string ToString(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an <see cref="Int64"/> value to a string that can be stored in Redis.
        /// </summary>
        /// <param name="value">The <see cref="Int64"/> value to convert.</param>
        /// <returns>A string representation of the specified <see cref="Int64"/> value that can be stored in Redis.</returns>
        public static string ToString(long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an <see cref="Single"/> value to a string that can be stored in Redis.
        /// </summary>
        /// <param name="value">The <see cref="Single"/> value to convert.</param>
        /// <returns>A string representation of the specified <see cref="Single"/> value that can be stored in Redis.</returns>
        public static string ToString(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an <see cref="Double"/> value to a string that can be stored in Redis.
        /// </summary>
        /// <param name="value">The <see cref="Double"/> value to convert.</param>
        /// <returns>A string representation of the specified <see cref="Double"/> value that can be stored in Redis.</returns>
        public static string ToString(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a <see cref="Guid"/> value to a string that can be stored in Redis.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> value to convert.</param>
        /// <returns>A string representation of the specified <see cref="Guid"/> value that can be stored in Redis.</returns>
        public static string ToString(Guid value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts an <see cref="DateTime"/> value to a string that can be stored in Redis.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> value to convert.</param>
        /// <returns>A string representation of the specified <see cref="DateTime"/> value that can be stored in Redis.</returns>
        public static string ToString(DateTime value)
        {
            return value.Ticks.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an <see cref="TimeSpan"/> value to a string that can be stored in Redis.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> value to convert.</param>
        /// <returns>A string representation of the specified <see cref="TimeSpan"/> value that can be stored in Redis.</returns>
        public static string ToString(TimeSpan value)
        {
            return value.Ticks.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an object to a string that can be stored in Redis.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <returns>A string representation of the specified object that can be stored in Redis.</returns>
        /// <exception cref="NotSupportedException">The type of the specified object is not supported.</exception>
        public static string ToString(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            Type type = value.GetType();

            if (type == typeof(string))
            {
                return (string)value;
            }

            if (type == typeof(int))
            {
                return RedisConverter.ToString((int)value);
            }

            if (type == typeof(long))
            {
                return RedisConverter.ToString((long)value);
            }

            if (type == typeof(float))
            {
                return RedisConverter.ToString((float)value);
            }

            if (type == typeof(double))
            {
                return RedisConverter.ToString((double)value);
            }

            if (type == typeof(Guid))
            {
                return RedisConverter.ToString((Guid)value);
            }

            if (type == typeof(TimeSpan))
            {
                return RedisConverter.ToString((TimeSpan)value);
            }

            if (type == typeof(DateTime))
            {
                return RedisConverter.ToString((DateTime)value);
            }

            if (type == typeof(bool))
            {
                return RedisConverter.ToString((bool)value);
            }

            if (type == typeof(Version))
            {
                return RedisConverter.ToString((Version)value);
            }

            if (type.IsEnum)
            {
                return value.ToString();
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return RedisConverter.ToString(((IEnumerable)value).Cast<object>().ToArray());
            }

            throw new NotSupportedException<Type>(type);
        }

        /// <summary>
        /// Converts a collection to a string that can be stored in Redis.
        /// </summary>
        /// <param name="collection">The collection to convert.</param>
        /// <returns>A string representation of the specified collection that can be stored in Redis.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="collection"/> is null.</exception>
        public static string ToString(params object[] collection)
        {
            if (collection == null)
            {
                return string.Empty;
            }

            return string.Join(RedisConverter.ArraySeparatorAsString, collection.Select(c => RedisConverter.ToString(c)));
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Boolean"/> value.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Boolean"/> value.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> cannot be parsed to <see cref="Boolean"/>.</exception>
        public static bool ParseBoolean(string value)
        {
            try
            {
                return bool.Parse(value);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Value '{0}' cannot be parsed to {1}.".FormatInvariant(value, typeof(bool)), "value", ex);
            }
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Int32"/> value.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Int32"/> value.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> cannot be parsed to <see cref="Int32"/>.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "integer", Justification = "This method parses a string to an integer.")]
        public static int ParseInteger(string value)
        {
            try
            {
                return int.Parse(value, NumberStyles.Integer | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Value '{0}' cannot be parsed to {1}.".FormatInvariant(value, typeof(long)), "value", ex);
            }
            catch (OverflowException ex)
            {
                throw new ArgumentException("Value '{0}' cannot be parsed to {1}.".FormatInvariant(value, typeof(long)), "value", ex);
            }
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Nullable{Int32}"/> value, if string is not null or empty. If the string is null or empty, returns 0.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Nullable{Int32}"/> value.</returns>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> is neither null nor empty and cannot be parsed to <see cref="Nullable{Int32}"/>.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "integer", Justification = "This method parses a string to an integer.")]
        public static int ParseIntegerOrDefault(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(int);
            }
            else
            {
                return RedisConverter.ParseInteger(value);
            }
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Nullable{Double}"/> value, if string is not null or empty. If the string is null or empty, returns 0.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Nullable{Double}"/> value.</returns>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> is neither null nor empty and cannot be parsed to <see cref="Nullable{Double}"/>.</exception>
        public static double ParseDoubleOrDefault(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(double);
            }
            else
            {
                return RedisConverter.ParseDouble(value);
            }
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Nullable{Int32}"/> value, if string is not null or empty. If the string is null or empty, returns null.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Nullable{Int32}"/> value.</returns>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> is neither null nor empty and cannot be parsed to <see cref="Nullable{Int32}"/>.</exception>
        public static int? ParseIntegerOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            else
            {
                return RedisConverter.ParseInteger(value);
            }
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Int64"/> value.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Int64"/> value.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> cannot be parsed to <see cref="Int64"/>.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long", Justification = "This method parses a string to a long.")]
        public static long ParseLong(string value)
        {
            try
            {
                return long.Parse(value, NumberStyles.Integer | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Value '{0}' cannot be parsed to {1}.".FormatInvariant(value, typeof(long)), "value", ex);
            }
            catch (OverflowException ex)
            {
                throw new ArgumentException("Value '{0}' cannot be parsed to {1}.".FormatInvariant(value, typeof(long)), "value", ex);
            }
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Single"/> value.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Single"/> value.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> cannot be parsed to <see cref="Single"/>.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float", Justification = "This method parses a string to a float.")]
        public static float ParseFloat(string value)
        {
            try
            {
                return float.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Value '{0}' cannot be parsed to {1}.".FormatInvariant(value, typeof(float)), "value", ex);
            }
            catch (OverflowException ex)
            {
                throw new ArgumentException("Value '{0}' cannot be parsed to {1}.".FormatInvariant(value, typeof(float)), "value", ex);
            }
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Double"/> value.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Double"/> value.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> cannot be parsed to <see cref="Double"/>.</exception>
        public static double ParseDouble(string value)
        {
            try
            {
                return double.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Value '{0}' cannot be parsed to {1}.".FormatInvariant(value, typeof(float)), "value", ex);
            }
            catch (OverflowException ex)
            {
                throw new ArgumentException("Value '{0}' cannot be parsed to {1}.".FormatInvariant(value, typeof(float)), "value", ex);
            }
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Guid"/> value.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Guid"/> value.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> cannot be parsed to <see cref="Guid"/>.</exception>
        public static Guid ParseGuid(string value)
        {
            try
            {
                return Guid.Parse(value);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Cannot parse '{0}' to '{1}'.".FormatInvariant(value, typeof(Guid)), "value", ex);
            }
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Nullable{Guid}"/> value, if string is not null or empty. If the string is null or empty, returns null.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Nullable{Guid}"/> value.</returns>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> is neither null nor empty and cannot be parsed to <see cref="Nullable{Guid}"/>.</exception>
        public static Guid? ParseGuidOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            else
            {
                return RedisConverter.ParseGuid(value);
            }
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="DateTime"/> value.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> cannot be parsed to <see cref="DateTime"/>.</exception>
        public static DateTime ParseDateTime(string value)
        {
            long ticks = RedisConverter.ParseLong(value);

            return new DateTime(ticks);
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Nullable{DateTime}"/> value, if string is not null or empty. If the string is null or empty, returns null.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Nullable{DateTime}"/> value.</returns>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> is neither null nor empty and cannot be parsed to <see cref="Nullable{DateTime}"/>.</exception>
        public static DateTime? ParseDateTimeOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            else
            {
                return RedisConverter.ParseDateTime(value);
            }
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="TimeSpan"/> value.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="TimeSpan"/> value.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> cannot be parsed to <see cref="TimeSpan"/>.</exception>
        public static TimeSpan ParseTimeSpan(string value)
        {
            long ticks = RedisConverter.ParseLong(value);

            return TimeSpan.FromTicks(ticks);
        }

        /// <summary>
        /// Parses a Redis string to an enumeration value.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type.</typeparam>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed enumeration value.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> cannot be parsed to <typeparamref name="TEnum"/>.</exception>
        public static TEnum ParseEnum<TEnum>(string value)
        {
            return (TEnum)RedisConverter.ParseEnum(value, typeof(TEnum));
        }

        /// <summary>
        /// Parses a Redis string to an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to parse.</typeparam>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed object.</returns>
        /// <exception cref="NotSupportedException">Type <typeparamref name="T" /> is not supported.</exception>
        public static T ParseValue<T>(string value)
        {
            return (T)RedisConverter.ParseValue(value, typeof(T));
        }

        /// <summary>
        /// Parses a Redis string to an object.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <param name="toType">The type of the object to parse.</param>
        /// <returns>The parsed object.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="toType"/> is null.</exception>
        /// <exception cref="NotSupportedException">Type <paramref name="toType" /> is not supported.</exception>
        public static object ParseValue(string value, Type toType)
        {
            if (toType == null)
            {
                throw new ArgumentNullException("toType");
            }

            if (toType == typeof(string))
            {
                return value;
            }

            if (toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }

                toType = toType.GetGenericArguments()[0];
            }

            if (toType == typeof(int))
            {
                return RedisConverter.ParseInteger(value);
            }

            if (toType == typeof(long))
            {
                return RedisConverter.ParseLong(value);
            }

            if (toType == typeof(float))
            {
                return RedisConverter.ParseFloat(value);
            }

            if (toType == typeof(double))
            {
                return RedisConverter.ParseDouble(value);
            }

            if (toType == typeof(Guid))
            {
                return RedisConverter.ParseGuid(value);
            }

            if (toType == typeof(TimeSpan))
            {
                return RedisConverter.ParseTimeSpan(value);
            }

            if (toType == typeof(DateTime))
            {
                return RedisConverter.ParseDateTime(value);
            }

            if (toType == typeof(bool))
            {
                return RedisConverter.ParseBoolean(value);
            }

            if (toType.IsEnum)
            {
                return RedisConverter.ParseEnum(value, toType);
            }

            if (toType == typeof(Type))
            {
                return RedisConverter.ParseType(value);
            }

            if (toType == typeof(Version))
            {
                return RedisConverter.ParseVersion(value);
            }

            throw new NotSupportedException<Type>(toType);
        }

        /// <summary>
        /// Parses a Redis string to a strongly-typed collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed strongly-typed collection.</returns>
        /// <exception cref="ArgumentException">Parameter <paramref name="value"/> cannot be parsed to a collection of <typeparamref name="T"/>.</exception>
        public static IEnumerable<T> ParseCollection<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Enumerable.Empty<T>();
            }

            return value
                .Split(RedisConverter.ArraySeparatorAsChar)
                .Select(v => RedisConverter.ParseValue<T>(v));
        }

        /// <summary>
        /// Converts a string to a Redis string.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>A representation of the string to be safely stored in Redis.</returns>
        public static string ToString(string value)
        {
            return value ?? string.Empty;
        }

        /// <summary>
        /// Converts a Type to a Redis string.
        /// </summary>
        /// <remarks>This method ignores the assembly version in order to maintain types between different releases.</remarks>
        /// <param name="value">The Type to convert.</param>
        /// <param name="includeAssemblyDetails">Whether to include the assembly version, culture and public key.</param>
        /// <returns>A representation of the Type to be safely stored in Redis.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        public static string ToString(Type value, bool includeAssemblyDetails)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (includeAssemblyDetails)
            {
                return value.AssemblyQualifiedName;
            }

            return string.Concat(value.FullName, ", ", value.Assembly.GetName().Name);
        }

        /// <summary>
        /// Converts a Version to a Redis string.
        /// </summary>
        /// <param name="value">The Version to convert.</param>
        /// <returns>A representation of the Version to be safely stored in Redis.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        public static string ToString(Version value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return value.ToString();
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Type"/> object.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Type"/> object.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        /// <exception cref="Exception">Parameter <paramref name="value"/> cannot be parsed to a valid <see cref="Type"/>.</exception>
        public static Type ParseType(string value)
        {
            try
            {
                return Type.GetType(value, true);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to parse type from string '{0}'.".FormatInvariant(value), "value", ex);
            }
        }

        /// <summary>
        /// Parses a Redis string to a <see cref="Version"/> object.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed <see cref="Version"/> object.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null.</exception>
        /// <exception cref="Exception">Parameter <paramref name="value"/> cannot be parsed to a valid <see cref="Version"/>.</exception>
        public static Version ParseVersion(string value)
        {
            try
            {
                return Version.Parse(value);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to parse version from string '{0}'.".FormatInvariant(value), "value", ex);
            }
        }

        private static object ParseEnum(string value, Type enumType)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            try
            {
                return Enum.Parse(enumType, value);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Value '{0}' cannot be parsed to {1}.".FormatInvariant(value, enumType), "value", ex);
            }
            catch (OverflowException ex)
            {
                throw new ArgumentException("Value '{0}' cannot be parsed to {1}.".FormatInvariant(value, enumType), "value", ex);
            }
        }
    }
}