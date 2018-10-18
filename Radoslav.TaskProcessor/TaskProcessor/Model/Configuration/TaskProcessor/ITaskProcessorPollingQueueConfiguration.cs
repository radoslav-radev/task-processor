namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic functionality of a polling queue configuration.
    /// </summary>
    public interface ITaskProcessorPollingQueueConfiguration : IPollingConfiguration
    {
        /// <summary>
        /// Gets the unique polling queue key.
        /// </summary>
        /// <value>The unique polling queue unique key.</value>
        string Key { get; }

        /// <summary>
        /// Gets or sets the number of the task workers that can be executed in parallel.
        /// </summary>
        /// <value>The number of the task workers that can be executed in parallel.</value>
        int MaxWorkers { get; set; }
    }
}