using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Event arguments for <see cref="ITaskProcessorMessageBus"/> task events.
    /// </summary>
    public class TaskEventArgs : EventArgs
    {
        private readonly Guid taskId;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskEventArgs"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        public TaskEventArgs(Guid taskId)
        {
            this.taskId = taskId;
        }

        /// <summary>
        /// Gets the ID of the task.
        /// </summary>
        /// <value>The ID of the task.</value>
        public Guid TaskId
        {
            get { return this.taskId; }
        }
    }
}