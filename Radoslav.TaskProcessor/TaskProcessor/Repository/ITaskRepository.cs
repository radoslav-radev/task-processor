using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository
{
    /// <summary>
    /// Basic functionality of <see cref="ITask"/> repository.
    /// </summary>
    public interface ITaskRepository
    {
        /// <summary>
        /// Retrieves a task from storage.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <returns>The task with the specified ID, or null if not found in storage.</returns>
        ITask GetById(Guid taskId);

        /// <summary>
        /// Adds a task to the storage.
        /// </summary>
        /// <param name="taskId">The ID of the task to add.</param>
        /// <param name="task">The task to add.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="task"/> is null.</exception>
        void Add(Guid taskId, ITask task);

        /// <summary>
        /// Deletes a task from storage but keeps its summary.
        /// </summary>
        /// <param name="taskId">The ID of the task to be deleted.</param>
        void Delete(Guid taskId);
    }
}