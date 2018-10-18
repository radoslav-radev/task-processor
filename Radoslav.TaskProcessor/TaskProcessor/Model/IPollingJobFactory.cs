using System;

namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// Basic functionality of a polling job factory.
    /// </summary>
    public interface IPollingJobFactory
    {
        /// <summary>
        /// Creates a polling job of the specified type.
        /// </summary>
        /// <param name="pollingJobType">The polling job type.</param>
        /// <returns>An instance of the <paramref name="pollingJobType"/>.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pollingJobType"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">Type <paramref name="pollingJobType"/> does not implement <see cref="IPollingJob"/>.</exception>
        /// <exception cref="NotSupportedException">Type <paramref name="pollingJobType"/> is not supported.</exception>
        /// <exception cref="Exception">Failed to instantiate a polling job with the specified type.</exception>
        IPollingJob Create(Type pollingJobType);
    }
}