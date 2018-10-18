using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Event arguments for <see cref="ITaskProcessorMessageBus"/> task processor events.
    /// </summary>
    public class TaskProcessorEventArgs : EventArgs
    {
        private readonly Guid taskProcessorId;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProcessorEventArgs"/> class.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor.</param>
        public TaskProcessorEventArgs(Guid taskProcessorId)
        {
            this.taskProcessorId = taskProcessorId;
        }

        /// <summary>
        /// Gets the ID of the task processor.
        /// </summary>
        /// <value>The ID of the task processor.</value>
        public Guid TaskProcessorId
        {
            get { return this.taskProcessorId; }
        }
    }
}