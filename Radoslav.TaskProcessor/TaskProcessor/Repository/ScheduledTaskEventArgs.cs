using System;

namespace Radoslav.TaskProcessor.Repository
{
    /// <summary>
    /// Event arguments for <see cref="IScheduledTaskRepository"/> events.
    /// </summary>
    public sealed class ScheduledTaskEventArgs : EventArgs
    {
        private readonly Guid scheduledTaskId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTaskEventArgs"/> class.
        /// </summary>
        /// <param name="scheduledTaskId">The ID of the scheduled task which has been added, changed or deleted.</param>
        public ScheduledTaskEventArgs(Guid scheduledTaskId)
        {
            this.scheduledTaskId = scheduledTaskId;
        }

        /// <summary>
        /// Gets the ID of the scheduled task which has been added, changed or deleted.
        /// </summary>
        /// <value>The ID of the scheduled task which has been added, changed or deleted.</value>
        public Guid ScheduledTaskId
        {
            get { return this.scheduledTaskId; }
        }
    }
}