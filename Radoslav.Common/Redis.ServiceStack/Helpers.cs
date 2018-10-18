using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ServiceStack;
using ServiceStack.Text;

namespace Radoslav.Redis.ServiceStack
{
    internal static class ServiceStackHelpers
    {
        private static readonly DateTime BeginningOfUnixEra = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        internal static DateTime ConvertDateTime(byte[][] time)
        {
            string unixTimeAsString = string.Concat(time[0].Select(v => v - 48));
            string microSecondsAsString = string.Concat(time[1].Select(v => v - 48));

            DateTime result = ServiceStackHelpers.BeginningOfUnixEra
                .AddSeconds(int.Parse(unixTimeAsString, CultureInfo.InvariantCulture))
                .AddMilliseconds(int.Parse(microSecondsAsString, CultureInfo.InvariantCulture) / 1000);

            Trace.WriteLine("EXIT: Server date time in UTC is {0}.".FormatInvariant(result));

            return result;
        }

        internal static RedisKeyType ConvertKeyType(global::ServiceStack.Redis.RedisKeyType keyType)
        {
            switch (keyType)
            {
                case global::ServiceStack.Redis.RedisKeyType.None:
                    return RedisKeyType.None;

                case global::ServiceStack.Redis.RedisKeyType.String:
                    return RedisKeyType.String;

                case global::ServiceStack.Redis.RedisKeyType.List:
                    return RedisKeyType.List;

                case global::ServiceStack.Redis.RedisKeyType.Set:
                    return RedisKeyType.Set;

                case global::ServiceStack.Redis.RedisKeyType.SortedSet:
                    return RedisKeyType.SortedSet;

                case global::ServiceStack.Redis.RedisKeyType.Hash:
                    return RedisKeyType.Hash;

                default:
                    throw new NotSupportedException<global::ServiceStack.Redis.RedisKeyType>(keyType);
            }
        }

        internal static Dictionary<string, string> ToStringDictionary(this byte[][] array)
        {
            return array.ToDictionary(content => content.FromUtf8Bytes());
        }

        internal static Dictionary<string, byte[]> ToBinaryDictionary(this byte[][] array)
        {
            return array.ToDictionary(content => content);
        }

        private static Dictionary<string, TValue> ToDictionary<TValue>(this byte[][] array, Func<byte[], TValue> converter)
        {
            Dictionary<string, TValue> result = new Dictionary<string, TValue>();

            for (int i = 0; i < array.Length; i += 2)
            {
                string field = array[i].FromUtf8Bytes();

                result[field] = converter(array[i + 1]);
            }

            return result;
        }
    }
}