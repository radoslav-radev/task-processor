using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.Repository
{
    /// <summary>
    /// Basic functionality of a repository for scheduled tasks.
    /// </summary>
    public interface IScheduledTaskRepository
    {
        /// <summary>
        /// An even raised when a scheduled task has been added.
        /// </summary>
        event EventHandler<ScheduledTaskEventArgs> Added;

        /// <summary>
        /// An even raised when a scheduled task has been updated.
        /// </summary>
        event EventHandler<ScheduledTaskEventArgs> Updated;

        /// <summary>
        /// An even raised when a scheduled task has been deleted.
        /// </summary>
        event EventHandler<ScheduledTaskEventArgs> Deleted;

        /// <summary>
        /// Gets or sets a value indicating whether to monitor for changes in storage and raise
        /// <see cref="Added"/>, <see cref="Updated"/> and <see cref="Deleted"/> events.
        /// </summary>
        /// <value>Whether to monitor for changes in storage and raise events.</value>
        bool RaiseEvents { get; set; }

        /// <summary>
        /// Gets all scheduled tasks from storage.
        /// </summary>
        /// <returns>All scheduled tasks from storage.</returns>
        IEnumerable<IScheduledTask> GetAll();

        /// <summary>
        /// Gets a scheduled task from storage.
        /// </summary>
        /// <param name="scheduledTaskId">The ID of the scheduled task to retrieve.</param>
        /// <returns>The scheduled task with the specified ID, or null if not found.</returns>
        IScheduledTask GetById(Guid scheduledTaskId);

        /// <summary>
        /// Adds a scheduled task to storage.
        /// </summary>
        /// <param name="scheduledTask">The scheduled task to add.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="scheduledTask"/> is null, or its <see cref="IScheduledTask.Schedule"/> is null.</exception>
        void Add(IScheduledTask scheduledTask);

        /// <summary>
        /// Updates a scheduled task in storage.
        /// </summary>
        /// <param name="scheduledTask">The scheduled task to update.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="scheduledTask"/> is null, or its <see cref="IScheduledTask.Schedule"/> is null.</exception>
        void Update(IScheduledTask scheduledTask);

        /// <summary>
        /// Removes a scheduled task from storage.
        /// </summary>
        /// <param name="scheduledTaskId">The ID of the scheduled task to remove.</param>
        void Delete(Guid scheduledTaskId);
    }
}