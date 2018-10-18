using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Event arguments for <see cref="ITaskProcessorMessageBus"/> task events.
    /// </summary>
    public class TaskEventEventArgs : TaskEventArgs
    {
        private readonly DateTime timestampUtc;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskEventEventArgs"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="timestampUtc">When the task event has happened, in UTC.</param>
        public TaskEventEventArgs(Guid taskId, DateTime timestampUtc)
            : base(taskId)
        {
            this.timestampUtc = timestampUtc;
        }

        /// <summary>
        /// Gets when the task event has happened, in UTC.
        /// </summary>
        /// <value>When the task event has happened in UTC.</value>
        public DateTime TimestampUtc
        {
            get { return this.timestampUtc; }
        }
    }
}