using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository
{
    /// <summary>
    /// Defines basic functionality of <see cref="ITaskRuntimeInfo"/> repository.
    /// </summary>
    public interface ITaskRuntimeInfoRepository
    {
        /// <summary>
        /// Gets from storage runtime information for all pending tasks.
        /// </summary>
        /// <param name="includePollingQueueTasks">Whether to include the pending polling queue tasks in the result.</param>
        /// <returns>Runtime information for all pending tasks in storage.</returns>
        IEnumerable<ITaskRuntimeInfo> GetPending(bool includePollingQueueTasks);

        /// <summary>
        /// Gets runtime information for all active tasks including the polling queue tasks.
        /// </summary>
        /// <returns>Runtime information for all active tasks including the polling queue tasks.</returns>
        IEnumerable<ITaskRuntimeInfo> GetActive();

        /// <summary>
        /// Gets runtime information for all pending tasks (excluding the polling queue tasks) and all active tasks (including the polling queue tasks).
        /// </summary>
        /// <returns>Runtime information for all pending and active tasks that are not in a polling queue.</returns>
        IReadOnlyDictionary<TaskStatus, IEnumerable<ITaskRuntimeInfo>> GetPendingAndActive();

        /// <summary>
        /// Gets runtime information for all failed tasks.
        /// </summary>
        /// <returns>Runtime information for all failed tasks.</returns>
        IEnumerable<ITaskRuntimeInfo> GetFailed();

        /// <summary>
        /// Gets runtime information for all tasks in the archive.
        /// </summary>
        /// <returns>Runtime information for all tasks in the archive.</returns>
        IEnumerable<ITaskRuntimeInfo> GetArchive();

        /// <summary>
        /// Gets runtime information for a specified number of pending tasks in a polling queue
        /// and removes them from the list of pending tasks for the polling queue.
        /// </summary>
        /// <param name="pollingQueueKey">The key of the polling queue.</param>
        /// <param name="maxResults">How many tasks to reserve.</param>
        /// <returns>Runtime information for the reserved pending tasks.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pollingQueueKey"/> is null or empty string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="maxResults"/> is a negative number.</exception>
        IEnumerable<ITaskRuntimeInfo> ReservePollingQueueTasks(string pollingQueueKey, int maxResults);

        /// <summary>
        /// Gets the task runtime information for a specified task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <returns>The task runtime formation for a specified task, or null if not found in storage.</returns>
        ITaskRuntimeInfo GetById(Guid taskId);

        /// <summary>
        /// Gets the type of a task.
        /// </summary>
        /// <param name="taskId">The ID of the task whose type to return.</param>
        /// <returns>The type of the specified task, or null if not found.</returns>
        Type GetTaskType(Guid taskId);

        /// <summary>
        /// Creates a runtime information for a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="taskType">The type of the task.</param>
        /// <param name="submittedUtc">When the task has been submitted, in UTC.</param>
        /// <param name="priority">The priority of the task.</param>
        /// <param name="pollingQueue">The key of the polling queue where the task belongs. If the task is not a polling queue task, value should be null.</param>
        /// <returns>A runtime information for the specified task.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="taskType"/> does not implement <see cref="ITask"/>.</exception>
        ITaskRuntimeInfo Create(Guid taskId, Type taskType, DateTime submittedUtc, TaskPriority priority, string pollingQueue);

        /// <summary>
        /// Add in storage a runtime information for a task.
        /// </summary>
        /// <param name="taskInfo">The task runtime information to add.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskInfo"/> is null.</exception>
        void Add(ITaskRuntimeInfo taskInfo);

        /// <summary>
        /// Updates in storage the runtime information for a task that has been assigned to a task processor.
        /// </summary>
        /// <param name="taskId">The ID of the task that has been assigned to a task processor.</param>
        /// <param name="taskProcessorId">The ID of the task processor to which the task has been assigned.</param>
        void Assign(Guid taskId, Guid? taskProcessorId);

        /// <summary>
        /// Updates in storage the runtime information for a task that has been started by a task processor.
        /// </summary>
        /// <param name="taskId">The ID of the task that has been started by a task processor.</param>
        /// <param name="taskProcessorId">The ID of the task processor that has started the task.</param>
        /// <param name="timestampUtc">When the task has been started by a task processor, in UTC.</param>
        void Start(Guid taskId, Guid taskProcessorId, DateTime timestampUtc);

        /// <summary>
        /// Updates in storage the runtime information for a task that has progressed.
        /// </summary>
        /// <remarks>This method is for performance optimization in order to update a task without retrieving it because the method is called very often.</remarks>
        /// <param name="taskId">The ID of the task that has progressed.</param>
        /// <param name="percentage">The percent of completion.</param>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="percentage"/> is less than 0 or greater than 100.</exception>
        void Progress(Guid taskId, double percentage);

        /// <summary>
        /// Update in storage the runtime information for a task that has been canceled by a client.
        /// </summary>
        /// <param name="taskId">The ID of the task that has been canceled.</param>
        /// <param name="timestampUtc">When the task has been canceled, in UTC.</param>
        void RequestCancel(Guid taskId, DateTime timestampUtc);

        /// <summary>
        /// Update in storage the runtime information for a task that has been canceled in the task processor executing it.
        /// </summary>
        /// <param name="taskId">The ID of the task that has been canceled.</param>
        /// <param name="timestampUtc">When the task processor stopped to execute the task, in UTC.</param>
        void CompleteCancel(Guid taskId, DateTime timestampUtc);

        /// <summary>
        /// Update in storage the runtime information for a task that has failed.
        /// </summary>
        /// <param name="taskId">The ID of the task that has failed.</param>
        /// <param name="timestampUtc">When the task has failed, in UTC.</param>
        /// <param name="error">The error that occurred during task execution.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="error"/> is null.</exception>
        void Fail(Guid taskId, DateTime timestampUtc, Exception error);

        /// <summary>
        /// Update in storage the runtime information for a task has completed successfully.
        /// </summary>
        /// <param name="taskId">The ID of the task that has completed successfully.</param>
        /// <param name="timestampUtc">When the task has completed successfully, in UTC.</param>
        void Complete(Guid taskId, DateTime timestampUtc);

        /// <summary>
        /// Checks whether a task is pending or in progress.
        /// </summary>
        /// <param name="taskId">The ID of the task to check.</param>
        /// <returns>True if task is pending or in progress; false if task has been canceled, failed or completed.</returns>
        bool CheckIsPendingOrActive(Guid taskId);
    }
}