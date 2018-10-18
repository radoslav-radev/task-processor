using System;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic functionality of a configuration for a task processor client.
    /// </summary>
    public interface ITaskProcessorClientConfiguration
    {
        /// <summary>
        /// Gets the polling queue key for tasks of a specified type.
        /// </summary>
        /// <param name="taskType">The type of the tasks for which to return the polling queue key.</param>
        /// <returns>The polling queue key for tasks of the specified type.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="taskType" /> is not a descendant of
        /// <see cref="Radoslav.TaskProcessor.Model.ITask"/>, or is not defined in the configuration.</exception>
        string GetPollingQueueKey(Type taskType);
    }
}