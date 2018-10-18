using System;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic definition of a configuration of scheduled tasks.
    /// </summary>
    public interface IScheduledTasksConfiguration
    {
        /// <summary>
        /// Gets the configuration for a scheduled task specified by its type.
        /// </summary>
        /// <param name="scheduledTaskType">The type of the scheduled task for which to return configuration.</param>
        /// <returns>Configuration for the specified scheduled task type, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="scheduledTaskType"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="scheduledTaskType"/> does not implement <see cref="Radoslav.TaskProcessor.TaskScheduler.IScheduledTask"/>.</exception>
        IScheduledTaskConfiguration this[Type scheduledTaskType] { get; }
    }
}