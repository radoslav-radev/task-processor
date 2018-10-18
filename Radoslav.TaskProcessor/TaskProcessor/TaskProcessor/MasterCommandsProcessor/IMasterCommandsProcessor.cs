using System;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Basic functionality of a master commands processor.
    /// </summary>
    public interface IMasterCommandsProcessor
    {
        /// <summary>
        /// Gets or sets a value indicating whether the master commands processor is active.
        /// </summary>
        /// <value>Whether the master commands processor is active.</value>
        bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the timeout after which task assignment to a task processor is considered failed and task is assigned to another task processor.
        /// </summary>
        /// <value>The timeout after which task assignment to a task processor is considered failed and task is assigned to another task processor.</value>
        /// <exception cref="ArgumentOutOfRangeException">Value is less than or equal to <see cref="TimeSpan.Zero"/>.</exception>
        TimeSpan AssignTaskTimeout { get; set; }

        /// <summary>
        /// Processes all available master commands in the order they are received.
        /// </summary>
        void ProcessMasterCommands();

        /// <summary>
        /// Cancels a task.
        /// </summary>
        /// <param name="taskId">The ID of the task that is being canceled.</param>
        void CancelTask(Guid taskId);
    }
}