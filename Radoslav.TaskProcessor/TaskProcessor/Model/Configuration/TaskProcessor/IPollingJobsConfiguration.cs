using System;
using System.Collections.Generic;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic functionality of polling jobs configuration.
    /// </summary>
    public interface IPollingJobsConfiguration : IEnumerable<IPollingJobConfiguration>
    {
        /// <summary>
        /// Gets the configuration for a polling job.
        /// </summary>
        /// <param name="implementationType">The type implementing <see cref="Radoslav.TaskProcessor.Model.IPollingJob"/>.</param>
        /// <returns>The configuration for the specified polling job type, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="implementationType"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="implementationType"/> is not a descendant of <see cref="Radoslav.TaskProcessor.Model.IPollingJob"/>.</exception>
        IPollingJobConfiguration this[Type implementationType] { get; }

        /// <summary>
        /// Adds a configuration for a polling job.
        /// </summary>
        /// <param name="implementationType">The type implementing <see cref="Radoslav.TaskProcessor.Model.IPollingJob"/>.</param>
        /// <returns>A configuration for the specified polling job.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="implementationType"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="implementationType"/> is not a descendant of <see cref="Radoslav.TaskProcessor.Model.IPollingJob"/>,
        /// or a polling job configuration for <paramref name="implementationType"/> already exists.</exception>
        IPollingJobConfiguration Add(Type implementationType);

        /// <summary>
        /// Creates a copy of another polling job configuration and adds it to the current collection.
        /// </summary>
        /// <param name="source">The polling job configuration to copy.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentException">A polling job configuration for <paramref name="source"/> <see cref="IPollingJobConfiguration.ImplementationType" /> already exists.</exception>
        void AddCopy(IPollingJobConfiguration source);

        /// <summary>
        /// Removes a polling job configuration.
        /// </summary>
        /// <param name="implementationType">The type implementing <see cref="Radoslav.TaskProcessor.Model.IPollingJob"/>.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="implementationType"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="implementationType"/> is not a descendant of <see cref="Radoslav.TaskProcessor.Model.IPollingJob"/>.</exception>
        void Remove(Type implementationType);
    }
}