using System;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic functionality of a task worker configuration.
    /// </summary>
    public interface ITaskWorkersConfiguration
    {
        /// <summary>
        /// Gets the task worker configuration for tasks of a specified type.
        /// </summary>
        /// <param name="taskType">The type of the task for which to return configuration.</param>
        /// <returns>The configuration of the task worker to be used to process tasks of a specified type, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="taskType"/> does not implement <see cref="Radoslav.TaskProcessor.Model.ITask"/>.</exception>
        ITaskWorkerConfiguration this[Type taskType] { get; }
    }
}