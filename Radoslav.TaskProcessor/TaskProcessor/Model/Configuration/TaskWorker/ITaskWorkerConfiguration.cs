using System;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic functionality of a single task worker configuration.
    /// </summary>
    public interface ITaskWorkerConfiguration
    {
        /// <summary>
        /// Gets the task type for which the task worker configuration should be applied.
        /// </summary>
        /// <value>The task type for which the task worker configuration should be applied.</value>
        Type TaskType { get; }

        /// <summary>
        /// Gets the type of the task worker to instantiate in order to processor the task.
        /// </summary>
        /// <value>The type of the task worker to instantiate in order to processor the task.</value>
        Type WorkerType { get; }

        /// <summary>
        /// Gets a value indicating whether the task worker expects task job settings.
        /// </summary>
        /// <value>Whether the task worker expects task job settings.</value>
        bool HasTaskJobSettings { get; }
    }
}