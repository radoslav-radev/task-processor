using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.TaskScheduler
{
    /// <summary>
    /// Basic functionality of a scheduled task based on recurrence definition.
    /// </summary>
    public interface IScheduledTask
    {
        /// <summary>
        /// Gets an unique ID for the scheduled task.
        /// </summary>
        /// <remarks>The ID is unique per type.</remarks>
        /// <value>Unique ID for the scheduled task.</value>
        Guid Id { get; }

        /// <summary>
        /// Gets or sets the task schedule definition.
        /// </summary>
        /// <value>The task schedule definition.</value>
        /// <exception cref="ArgumentNullException">Value is null.</exception>
        IScheduleDefinition Schedule { get; set; }

        /// <summary>
        /// Gets the next task to be submitted to the task processor.
        /// </summary>
        /// <value>The next task to be submitted to the task processor.</value>
        ITask NextTask { get; }

        /// <summary>
        /// Gets the priority of the next task to be submitted to the task processor.
        /// </summary>
        /// <value>The priority of the next task to be submitted to the task processor.</value>
        TaskPriority NextTaskPriority { get; }

        /// <summary>
        /// Gets the summary of the next task to be submitted to the task processor.
        /// </summary>
        /// <value>The summary of the next task to be submitted to the task processor.</value>
        ITaskSummary NextTaskSummary { get; }

        /// <summary>
        /// Prepares the next task, priority and summary to be submitted to the task processor.
        /// </summary>
        void PrepareNextTask();
    }
}