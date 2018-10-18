using System;
using System.Collections.Generic;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Defines the basic functionality of a polling queue configuration.
    /// </summary>
    public interface ITaskProcessorPollingQueuesConfiguration : IEnumerable<ITaskProcessorPollingQueueConfiguration>
    {
        /// <summary>
        /// Gets the configuration of a polling queue specified by its unique key.
        /// </summary>
        /// <param name="key">The unique key of the polling queue whose configuration should be retrieved.</param>
        /// <returns>The configuration for the polling queue with the specified key, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty.</exception>
        ITaskProcessorPollingQueueConfiguration this[string key] { get; }

        /// <summary>
        /// Adds a polling queue configuration specified by its unique key.
        /// </summary>
        /// <param name="key">The polling queue unique key.</param>
        /// <returns>The newly created and added polling queue configuration instance.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="key"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">A polling queue configuration for <paramref name="key"/> already exists.</exception>
        ITaskProcessorPollingQueueConfiguration Add(string key);

        /// <summary>
        /// Creates a copy of another polling queue configuration and adds it to the current polling queue configuration collection.
        /// </summary>
        /// <param name="source">The another polling queue configuration to copy.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentException">A polling queue configuration for <paramref name="source"/> <see cref="ITaskProcessorPollingQueueConfiguration.Key"/> already exists.</exception>
        void AddCopy(ITaskProcessorPollingQueueConfiguration source);

        /// <summary>
        /// Removes a polling queue configuration.
        /// </summary>
        /// <param name="key">The unique key of the polling queue configuration to remove.</param>
        void Remove(string key);
    }
}