using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository
{
    /// <summary>
    /// Basic implementation of a task summary repository.
    /// </summary>
    public interface ITaskSummaryRepository
    {
        /// <summary>
        /// Adds a task summary to storage.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="summary">The task summary.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="summary"/> is null.</exception>
        void Add(Guid taskId, ITaskSummary summary);

        /// <summary>
        /// Gets the summary of a task by its ID.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <returns>A summary for the specified task, or null if not found.</returns>
        ITaskSummary GetById(Guid taskId);
    }
}