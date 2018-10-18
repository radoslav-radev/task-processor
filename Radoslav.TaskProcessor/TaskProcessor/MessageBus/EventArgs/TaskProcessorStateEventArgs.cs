using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Event arguments for <see cref="ITaskProcessorMessageBusReceiver.StateChanged"/> event.
    /// </summary>
    public sealed class TaskProcessorStateEventArgs : TaskProcessorEventArgs
    {
        private readonly TaskProcessorState taskProcessorState;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProcessorStateEventArgs"/> class.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor whose state has changed.</param>
        /// <param name="taskProcessorState">The new state of the task processor.</param>
        public TaskProcessorStateEventArgs(Guid taskProcessorId, TaskProcessorState taskProcessorState)
            : base(taskProcessorId)
        {
            this.taskProcessorState = taskProcessorState;
        }

        /// <summary>
        /// Gets the state of the task processor.
        /// </summary>
        /// <value>The state of the task processor.</value>
        public TaskProcessorState TaskProcessorState
        {
            get { return this.taskProcessorState; }
        }
    }
}