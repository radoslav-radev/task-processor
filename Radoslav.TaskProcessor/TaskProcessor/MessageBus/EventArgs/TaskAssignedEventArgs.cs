using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Event arguments for the <see cref="ITaskMessageBusReceiver.TaskAssigned" /> event
    /// indicating that a task has been assigned to a task processor by the master task processor.
    /// </summary>
    public sealed class TaskAssignedEventArgs : TaskEventArgs
    {
        private readonly Guid taskProcessorId;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskAssignedEventArgs"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the assigned task.</param>
        /// <param name="taskProcessorId">The ID of the task processor to which the task has been assigned by the master task processor.</param>
        public TaskAssignedEventArgs(Guid taskId, Guid taskProcessorId)
            : base(taskId)
        {
            this.taskProcessorId = taskProcessorId;
        }

        /// <summary>
        /// Gets the ID of the task processor to which the task has been assigned by the master task processor.
        /// </summary>
        /// <value>The ID of the task processor to which the task has been assigned by the master task processor.</value>
        public Guid TaskProcessorId
        {
            get { return this.taskProcessorId; }
        }
    }
}