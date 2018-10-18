namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic functionality of a task processor configuration.
    /// </summary>
    public interface ITaskProcessorConfiguration
    {
        /// <summary>
        /// Gets the task jobs configuration.
        /// </summary>
        /// <remarks>This value is never null.</remarks>
        /// <value>The task jobs configuration.</value>
        ITaskJobsConfiguration Tasks { get; }

        /// <summary>
        /// Gets the polling queues configuration.
        /// </summary>
        /// <remarks>This value is never null.</remarks>
        /// <value>The polling queues configuration.</value>
        ITaskProcessorPollingQueuesConfiguration PollingQueues { get; }

        /// <summary>
        /// Gets the polling jobs configuration.
        /// </summary>
        /// <remarks>This value is never null.</remarks>
        /// <value>The polling jobs configuration.</value>
        IPollingJobsConfiguration PollingJobs { get; }
    }
}