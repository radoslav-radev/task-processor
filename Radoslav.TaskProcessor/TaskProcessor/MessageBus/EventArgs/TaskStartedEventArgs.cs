using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Event arguments for <see cref="ITaskMessageBusReceiver.TaskStarted"/> event.
    /// </summary>
    public sealed class TaskStartedEventArgs : TaskEventEventArgs
    {
        private readonly Guid taskProcessorId;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskStartedEventArgs"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the task that has been started.</param>
        /// <param name="taskProcessorId">The ID of the task processor that has started the task.</param>
        /// <param name="timestampUtc">When the task has been started, in UTC.</param>
        public TaskStartedEventArgs(Guid taskId, Guid taskProcessorId, DateTime timestampUtc)
            : base(taskId, timestampUtc)
        {
            this.taskProcessorId = taskProcessorId;
        }

        /// <summary>
        /// Gets the ID of the task processor that has started the task.
        /// </summary>
        /// <value>The ID of the task processor that has started the task.</value>
        public Guid TaskProcessorId
        {
            get { return this.taskProcessorId; }
        }
    }
}