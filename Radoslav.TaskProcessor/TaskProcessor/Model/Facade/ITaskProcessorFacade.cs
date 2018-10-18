using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.Facade
{
    /// <summary>
    /// Basic functionality of a task processor facade.
    /// </summary>
    public interface ITaskProcessorFacade
    {
        /// <summary>
        /// Submits a task to the task processor and returns an auto-generated unique ID for it.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        /// <returns>The auto-generated unique ID for the submitted task.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="task" /> is null.</exception>
        Guid SubmitTask(ITask task);

        /// <summary>
        /// Submits a task to the task processor and returns an auto-generated unique ID for it.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="summary">A short summary for the task (not mandatory).</param>
        /// <returns>The auto-generated unique ID for the submitted task.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="task" /> is null.</exception>
        Guid SubmitTask(ITask task, ITaskSummary summary);

        /// <summary>
        /// Submits a task to the task processor with a specified priority and returns an auto-generated unique ID for it.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="priority">The priority of the task.</param>
        /// <returns>The auto-generated unique ID for the submitted task.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="task" /> is null.</exception>
        /// <exception cref="InvalidOperationException">Parameter <paramref name="task"/> is a polling queue task but priority is not Normal.</exception>
        Guid SubmitTask(ITask task, TaskPriority priority);

        /// <summary>
        /// Submits a task to the task processor with a specified priority and returns an auto-generated unique ID for it.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="summary">A short summary for the task (not mandatory).</param>
        /// <param name="priority">The priority of the task.</param>
        /// <returns>The auto-generated unique ID for the submitted task.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="task" /> is null.</exception>
        Guid SubmitTask(ITask task, ITaskSummary summary, TaskPriority priority);

        /// <summary>
        /// Cancels a requested task.
        /// </summary>
        /// <param name="taskId">The ID of the task to cancel.</param>
        /// <exception cref="KeyNotFoundException">Task with the specified ID is not found.</exception>
        /// <exception cref="InvalidOperationException">The specified task has already been canceled, or has completed.</exception>
        void CancelTask(Guid taskId);

        /// <summary>
        /// Sends a command to a task processor to become master.
        /// </summary>
        /// <remarks>The current master also receives the command and becomes slave.</remarks>
        /// <param name="taskProcessorId">The ID of the task processor that should become master.</param>
        void MakeTaskProcessorMaster(Guid taskProcessorId);

        /// <summary>
        /// Sends a notification to a task processor to stop after all running tasks have finished.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor to stop.</param>
        void RequestTaskProcessorToStop(Guid taskProcessorId);

        /// <summary>
        /// Gets a task by ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to retrieve.</param>
        /// <returns>The task with the specified ID, or null if not found.</returns>
        ITask GetTask(Guid taskId);

        /// <summary>
        /// Gets runtime information for a task.
        /// </summary>
        /// <param name="taskId">The ID of the task to retrieve runtime information for.</param>
        /// <returns>Runtime information for the specified task, or null if not found.</returns>
        ITaskRuntimeInfo GetTaskRuntimeInfo(Guid taskId);

        /// <summary>
        /// Gets runtime information for all active task processors.
        /// </summary>
        /// <returns>A collection with the runtime information for all active task processors.</returns>
        IEnumerable<ITaskProcessorRuntimeInfo> GetTaskProcessorRuntimeInfo();

        /// <summary>
        /// Gets runtime information for a task processor.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor.</param>
        /// <returns>Runtime information for the specified task processor, or null if not found.</returns>
        ITaskProcessorRuntimeInfo GetTaskProcessorRuntimeInfo(Guid taskProcessorId);

        /// <summary>
        /// Updates runtime information for a task processor.
        /// </summary>
        /// <param name="taskProcessorInfo">The new runtime information for the task processor.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskProcessorInfo"/> is null.</exception>
        void UpdateTaskProcessorRuntimeInfo(ITaskProcessorRuntimeInfo taskProcessorInfo);

        /// <summary>
        /// Gets the ID of the task processor that is currently master.
        /// </summary>
        /// <returns>The ID of the task processor that is currently master.</returns>
        Guid? GetMasterTaskProcessorId();

        /// <summary>
        /// Gets the task job settings for a task type.
        /// </summary>
        /// <param name="taskType">The type of the task.</param>
        /// <returns>The task job settings for the specified type, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="taskType"/> does not implement <see cref="ITask"/>.</exception>
        ITaskJobSettings GetTaskJobSettings(Type taskType);

        /// <summary>
        /// Sets the task job settings for a task type.
        /// </summary>
        /// <param name="taskType">The type of the task.</param>
        /// <param name="settings">The task job settings for the specified task type.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType"/> or <paramref name="settings"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="taskType"/> does not implement <see cref="ITask"/>.</exception>
        void SetTaskJobSettings(Type taskType, ITaskJobSettings settings);

        /// <summary>
        /// Clears the task job settings for a task type.
        /// </summary>
        /// <param name="taskType">The type of the task.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType"/> is null or does not implement <see cref="ITask"/>.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="taskType"/> does not implement <see cref="ITask"/>.</exception>
        void ClearTaskJobSettings(Type taskType);

        /// <summary>
        /// Gets a summary for a task.
        /// </summary>
        /// <param name="taskId">The ID of the task to retrieve summary for.</param>
        /// <returns>A summary for the specified task. or null if not found.</returns>
        ITaskSummary GetTaskSummary(Guid taskId);

        /// <summary>
        /// Adds a scheduled task to storage.
        /// </summary>
        /// <param name="scheduledTask">The scheduled task to add.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="scheduledTask"/> is null.</exception>
        void AddScheduledTask(IScheduledTask scheduledTask);

        /// <summary>
        /// Updates a scheduled task in storage.
        /// </summary>
        /// <param name="scheduledTask">The scheduled task to update.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="scheduledTask"/> is null.</exception>
        void UpdateScheduledTask(IScheduledTask scheduledTask);

        /// <summary>
        /// Removes a scheduled task from storage.
        /// </summary>
        /// <param name="scheduledTaskId">The ID of the scheduled task to remove.</param>
        void DeleteScheduledTask(Guid scheduledTaskId);

        /// <summary>
        /// Creates a session for submitting a task.
        /// </summary>
        /// <returns>A session for submitting a task.</returns>
        ISubmitTaskSession CreateSubmitTaskSession();
    }
}