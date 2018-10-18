using System;
using System.Collections.Generic;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic functionality of a task jobs configuration.
    /// </summary>
    public interface ITaskJobsConfiguration : IEnumerable<ITaskJobConfiguration>
    {
        /// <summary>
        /// Gets or sets the number of the task workers that can be executed in parallel.
        /// </summary>
        /// <value>The number of the task workers that can be executed in parallel.</value>
        int? MaxWorkers { get; set; }

        /// <summary>
        /// Gets a configuration for a task job.
        /// </summary>
        /// <param name="taskType">The task job's <see cref="Type"/>. This must be an interface type
        /// descending from <see cref="Radoslav.TaskProcessor.Model.ITask"/>.</param>
        /// <returns>The configuration for the specified task job, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="taskType"/> is not an interface,
        /// or is not a descendant of <see cref="Radoslav.TaskProcessor.Model.ITask"/>.</exception>
        ITaskJobConfiguration this[Type taskType] { get; }

        /// <summary>
        /// Adds a task job configuration specified by its task type.
        /// </summary>
        /// <param name="taskType">The task job type.</param>
        /// <returns>The newly created and added task job configuration instance.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType"/> is null.</exception>
        ITaskJobConfiguration Add(Type taskType);

        /// <summary>
        /// Creates a copy of another task job configuration and adds it to the current task jobs configuration collection.
        /// </summary>
        /// <param name="source">The another task job configuration to copy.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="source"/> is null.</exception>
        void AddCopy(ITaskJobConfiguration source);

        /// <summary>
        /// Removes a task job configuration.
        /// </summary>
        /// <param name="taskType">The task job type.</param>
        void Remove(Type taskType);
    }
}