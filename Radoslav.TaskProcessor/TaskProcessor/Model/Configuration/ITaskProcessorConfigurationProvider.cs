namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic functionality of a task processor configuration provider.
    /// </summary>
    public interface ITaskProcessorConfigurationProvider
    {
        /// <summary>
        /// Gets a task processor configuration.
        /// </summary>
        /// <returns>A task processor configuration.</returns>
        ITaskProcessorConfiguration GetTaskProcessorConfiguration();

        /// <summary>
        /// Gets a task worker configuration.
        /// </summary>
        /// <returns>A task worker configuration.</returns>
        ITaskWorkersConfiguration GetTaskWorkerConfiguration();

        /// <summary>
        /// Gets a task processor client configuration.
        /// </summary>
        /// <returns>A task processor client configuration.</returns>
        ITaskProcessorClientConfiguration GetClientConfiguration();

        /// <summary>
        /// Gets a configuration for serialization.
        /// </summary>
        /// <returns>A configuration for serialization.</returns>
        ITaskProcessorSerializationConfiguration GetSerializationConfiguration();

        /// <summary>
        /// Gets a configuration for the task scheduler.
        /// </summary>
        /// <returns>A configuration for the task scheduler.</returns>
        ITaskSchedulerConfiguration GetTaskSchedulerConfiguration();
    }
}