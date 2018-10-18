using System;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic functionality of a task job configuration.
    /// </summary>
    public interface ITaskJobConfiguration
    {
        /// <summary>
        /// Gets the task job's type.
        /// </summary>
        /// <value>The type for the task job.</value>
        Type TaskType { get; }

        /// <summary>
        /// Gets or sets the number of the task workers that can be executed in parallel.
        /// </summary>
        /// <value>The number of the task workers that can be executed in parallel.</value>
        int? MaxWorkers { get; set; }
    }
}