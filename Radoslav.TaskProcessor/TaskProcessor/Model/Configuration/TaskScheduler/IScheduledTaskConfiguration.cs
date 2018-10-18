using System;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic definition of a configuration for a scheduled task.
    /// </summary>
    public interface IScheduledTaskConfiguration
    {
        /// <summary>
        /// Gets the type of the scheduled task for which the configuration applies.
        /// </summary>
        /// <value>The type of the scheduled task for which the configuration applies.</value>
        Type ScheduledTaskType { get; }

        /// <summary>
        /// Gets a value indicating whether the previous submitted task should have completed before submitting another one.
        /// </summary>
        /// <value>Whether the previous submitted task should have completed before submitting another one.</value>
        bool WaitForPreviousSubmittedTaskToComplete { get; }
    }
}