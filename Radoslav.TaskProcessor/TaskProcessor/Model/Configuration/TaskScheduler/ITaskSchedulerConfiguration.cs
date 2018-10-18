namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic definition of a task scheduler configuration.
    /// </summary>
    public interface ITaskSchedulerConfiguration
    {
        /// <summary>
        /// Gets the configuration for the scheduled tasks.
        /// </summary>
        /// <value>The configuration for the scheduled tasks.</value>
        IScheduledTasksConfiguration ScheduledTasks { get; }
    }
}