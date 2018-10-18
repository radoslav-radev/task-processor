using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository
{
    /// <summary>
    /// Basic functionality of a task job settings repository.
    /// </summary>
    public interface ITaskJobSettingsRepository
    {
        /// <summary>
        /// Gets task job settings for a task type.
        /// </summary>
        /// <param name="taskType">The type of the task.</param>
        /// <returns>Task job settings for the specified task type, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="taskType"/> does not implement <see cref="ITask"/>.</exception>
        ITaskJobSettings Get(Type taskType);

        /// <summary>
        /// Sets task job settings for a task type.
        /// </summary>
        /// <param name="taskType">The type of the task.</param>
        /// <param name="settings">The task job settings to set.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType" /> or <paramref name="settings" /> is null. </exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="taskType"/> does not implement <see cref="ITask"/>.</exception>
        void Set(Type taskType, ITaskJobSettings settings);

        /// <summary>
        /// Clears the task job settings for a task type.
        /// </summary>
        /// <param name="taskType">The type of the task.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType"/> is null or does not implement <see cref="ITask"/>.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="taskType"/> does not implement <see cref="ITask"/>.</exception>
        void Clear(Type taskType);
    }
}