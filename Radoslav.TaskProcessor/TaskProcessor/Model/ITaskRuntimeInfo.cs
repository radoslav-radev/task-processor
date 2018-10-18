using System;

namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// Basic functionality of a task runtime information.
    /// </summary>
    /// <remarks>These objects are used by the task processors during runtime and are stored in Redis or other cache.</remarks>
    public interface ITaskRuntimeInfo
    {
        /// <summary>
        /// Gets the ID of the task.
        /// </summary>
        /// <value>The ID of the task.</value>
        Guid TaskId { get; }

        /// <summary>
        /// Gets the type of the task.
        /// </summary>
        /// <value>The type of the task.</value>
        Type TaskType { get; }

        /// <summary>
        /// Gets the key of the polling queue where the task belongs.
        /// </summary>
        /// <value>The key of the polling queue where the task belongs.</value>
        string PollingQueue { get; }

        /// <summary>
        /// Gets the priority of the task.
        /// </summary>
        /// <value>The priority of the task.</value>
        TaskPriority Priority { get; }

        /// <summary>
        /// Gets the current status of the task.
        /// </summary>
        /// <value>The current status of the task.</value>
        TaskStatus Status { get; }

        /// <summary>
        /// Gets the ID of the task processor that executes the task.
        /// </summary>
        /// <value>The ID of the task processor that executes the task.</value>
        Guid? TaskProcessorId { get; }

        /// <summary>
        /// Gets the percent of completion of the task.
        /// </summary>
        /// <value>The percent of completion of the task.</value>
        double Percentage { get; }

        /// <summary>
        /// Gets when the task has been requested, in UTC.
        /// </summary>
        /// <value>The task has been requested, in UTC.</value>
        DateTime SubmittedUtc { get; }

        /// <summary>
        /// Gets when the task has started, in UTC.
        /// </summary>
        /// <remarks>If value is null, the task has not been started yet.</remarks>
        /// <value>When the task has started, in UTC.</value>
        DateTime? StartedUtc { get; }

        /// <summary>
        /// Gets when the task has been canceled, in UTC.
        /// </summary>
        /// <remarks>If value is null, the task has not been canceled.</remarks>
        /// <value>When the task has been canceled, in UTC.</value>
        DateTime? CanceledUtc { get; }

        /// <summary>
        /// Gets when the task has completed, in UTC.
        /// </summary>
        /// <remarks>If value is null, the task has not completed yet.</remarks>
        /// <value>When the task has completed, in UTC.</value>
        DateTime? CompletedUtc { get; }

        /// <summary>
        /// Gets the error that occurred during the task execution.
        /// </summary>
        /// <value>The error that occurred during the task execution.</value>
        string Error { get; }
    }
}