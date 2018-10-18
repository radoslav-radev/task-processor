using System;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic definition of a polling configuration.
    /// </summary>
    public interface IPollingConfiguration
    {
        /// <summary>
        /// Gets or sets the time interval between two polls.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="value"/> is not a positive <see cref="TimeSpan"/>.</exception>
        /// <value>The time interval between two polls.</value>
        TimeSpan PollInterval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the polling should be executed.
        /// </summary>
        /// <remarks>This value may be false if an administrator wants to configure a task processor instance not to process a certain polling job or polling queue.</remarks>
        /// <value>Whether the polling job should be executed.</value>
        bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the polling should be executed only by the master task processor.
        /// </summary>
        /// <value>Whether the polling job should be executed only by the master task processor.</value>
        bool IsMaster { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a next polling can be made before the previous one has completed.
        /// </summary>
        /// <value>Whether polling job executions must be always sequential or could be in parallel.</value>
        bool IsConcurrent { get; set; }
    }
}