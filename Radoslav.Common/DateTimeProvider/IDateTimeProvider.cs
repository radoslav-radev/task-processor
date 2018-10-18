using System;

namespace Radoslav.DateTimeProvider
{
    /// <summary>
    /// Basic functionality of a task processor date time provider.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets the current date and time, in UTC.
        /// </summary>
        /// <value>The current date and time in UTC.</value>
        DateTime UtcNow { get; }
    }
}