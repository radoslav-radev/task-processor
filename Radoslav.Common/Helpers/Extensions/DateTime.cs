using System;

namespace Radoslav
{
    public static partial class Helpers
    {
        private static readonly long OneHourTicks = TimeSpan.FromHours(1).Ticks;

        /// <summary>
        /// Gets a random date time value between <see cref="DateTime.MinValue"/> and <see cref="DateTime.MaxValue"/>.
        /// </summary>
        /// <returns>Random date time value between <see cref="DateTime.MinValue"/> and <see cref="DateTime.MaxValue"/>.</returns>
        public static DateTime GetRandomDateTime()
        {
            return new DateTime(Convert.ToInt64(Math.Round(Helpers.Randomizer.NextDouble() * DateTime.MaxValue.Ticks)), DateTimeKind.Utc);
        }

        /// <summary>
        /// Checks if a <see cref="TimeSpan"/> divides the hour in equal parts.
        /// </summary>
        /// <example>
        /// <code>
        /// TimeSpan.FromMinutes(1).IsHourDivider(); // Result is True
        /// TimeSpan.FromMinutes(5).IsHourDivider(); // Result is True
        /// TimeSpan.FromMinutes(10).IsHourDivider(); // Result is True
        /// TimeSpan.FromMinutes(20).IsHourDivider(); // Result is True
        /// TimeSpan.FromMinutes(13).IsHourDivider(); // Result is False
        /// TimeSpan.FromMinutes(29).IsHourDivider(); // Result is False
        /// </code>
        /// </example>
        /// <param name="value">The <see cref="TimeSpan"/> value to be checked.</param>
        /// <returns><c>true</c> if <paramref name="value"/> divides the hour in equal parts; otherwise <c>false</c>.</returns>
        public static bool IsHourDivider(this TimeSpan value)
        {
            return (value > TimeSpan.Zero) && (Helpers.OneHourTicks != value.Ticks) && (Helpers.OneHourTicks % value.Ticks == 0);
        }

        /// <summary>
        /// Multiplies a <see cref="TimeSpan"/> with a number.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> to be multiplied.</param>
        /// <param name="factor">The factor to multiple with.</param>
        /// <returns>The product of multiplication.</returns>
        public static TimeSpan Multiply(this TimeSpan value, double factor)
        {
            return TimeSpan.FromTicks(Convert.ToInt64(value.Ticks * factor));
        }

        /// <summary>
        /// Gets the beginning of a specified time interval relative to a specified <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> value.</param>
        /// <param name="interval">The time interval.</param>
        /// <returns>The beginning of the specified time interval.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="interval"/> is less than or equal to zero.</exception>
        /// <remarks>This method finds the beginning of the time interval in which belongs the specified <see cref="DateTime"/> <paramref name="value"/>,
        /// if the <see cref="TimeSpan"/> is divided in equal parts specified by the <paramref name="interval"/> parameter.</remarks>
        /// <example>
        /// <code>
        /// DateTime.Parse("2015/1/1 01:00:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 2015/1/1 01:00:00
        /// DateTime.Parse("2015/1/1 01:05:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 2015/1/1 01:00:00
        /// DateTime.Parse("2015/1/1 01:10:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 2015/1/1 01:00:00
        /// DateTime.Parse("2015/1/1 01:15:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 2015/1/1 01:15:00
        /// DateTime.Parse("2015/1/1 01:20:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 2015/1/1 01:15:00
        /// DateTime.Parse("2015/1/1 01:25:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 2015/1/1 01:15:00
        /// DateTime.Parse("2015/1/1 01:30:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 2015/1/1 01:30:00
        /// DateTime.Parse("2015/1/1 01:35:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 2015/1/1 01:30:00
        /// DateTime.Parse("2015/1/1 01:40:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 2015/1/1 01:30:00
        /// DateTime.Parse("2015/1/1 01:45:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 2015/1/1 01:45:00
        /// </code>
        /// </example>
        public static DateTime GetBeginningOfTimeInterval(this DateTime value, TimeSpan interval)
        {
            return value.Date + value.TimeOfDay.GetBeginningOfTimeInterval(interval);
        }

        /// <summary>
        /// Gets the beginning of a specified time interval relative to a specified <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> value.</param>
        /// <param name="interval">The time interval.</param>
        /// <returns>The beginning of the specified time interval.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="interval"/> is less than or equal to zero.</exception>
        /// <remarks>This method finds the beginning of the time interval in which belongs the specified <see cref="TimeSpan"/> <paramref name="value"/>,
        /// if the <see cref="TimeSpan"/> is divided in equal parts specified by the <paramref name="interval"/> parameter.</remarks>
        /// <example>
        /// <code>
        /// TimeSpan.Parse("01:00:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 01:00:00
        /// TimeSpan.Parse("01:05:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 01:00:00
        /// TimeSpan.Parse("01:10:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 01:00:00
        /// TimeSpan.Parse("01:15:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 01:15:00
        /// TimeSpan.Parse("01:20:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 01:15:00
        /// TimeSpan.Parse("01:25:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 01:15:00
        /// TimeSpan.Parse("01:30:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 01:30:00
        /// TimeSpan.Parse("01:35:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 01:30:00
        /// TimeSpan.Parse("01:40:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 01:30:00
        /// TimeSpan.Parse("01:45:00").GetBeginningOfTimeInterval(TimeSpan.Parse("00:15:00")); // Result is 01:45:00
        /// </code>
        /// </example>
        private static TimeSpan GetBeginningOfTimeInterval(this TimeSpan value, TimeSpan interval)
        {
            if (value < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("value", "Parameter should not be negative.");
            }

            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("interval", "Parameter should be positive.");
            }

            long distanceTicks = value.Ticks % interval.Ticks;

            return TimeSpan.FromTicks(value.Ticks - distanceTicks);
        }
    }
}