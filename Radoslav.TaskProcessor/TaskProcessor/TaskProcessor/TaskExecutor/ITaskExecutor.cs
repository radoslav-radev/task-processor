using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Basic implementation of task executor.
    /// </summary>
    /// <remarks>
    /// This class is responsible for executing tasks in threads, processes, etc.
    /// </remarks>
    public interface ITaskExecutor
    {
        /// <summary>
        /// A task has execution has completed because the task has been canceled.
        /// </summary>
        event EventHandler<TaskEventArgs> TaskCanceled;

        /// <summary>
        /// A task execution has failed.
        /// </summary>
        event EventHandler<TaskEventArgs> TaskFailed;

        /// <summary>
        /// A task execution has completed successfully.
        /// </summary>
        event EventHandler<TaskCompletedEventArgs> TaskCompleted;

        /// <summary>
        /// Gets the number of the currently running tasks.
        /// </summary>
        /// <value>The number of the currently running tasks.</value>
        int ActiveTasksCount { get; }

        /// <summary>
        /// Gets or sets the timeout before a task is killed by the task executor when cancel request is received.
        /// </summary>
        /// <value>The timeout before a task is killed by the task executor when when cancel request is received.</value>
        TimeSpan CancelTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to monitor tasks performance (CPU, memory, etc.).
        /// </summary>
        /// <value>Whether to monitor tasks performance (CPU, memory, etc.).</value>
        bool MonitorPerformance { get; set; }

        /// <summary>
        /// Starts a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="priority">The priority of the task.</param>
        void StartTask(Guid taskId, TaskPriority priority);

        /// <summary>
        /// Cancels a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        void CancelTask(Guid taskId);

        /// <summary>
        /// Gets performance information for the currently executed tasks.
        /// </summary>
        /// <returns>Performance information for the currently executed tasks.</returns>
        IEnumerable<TaskPerformanceReport> GetTasksPerformanceInfo();
    }
}