using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Event arguments for the <see cref="ITaskMessageBusReceiver.TaskCompleted" /> event indicating that a task has been completed.
    /// </summary>
    public sealed class TaskCompletedEventArgs : TaskEventEventArgs
    {
        private readonly TimeSpan totalCpuTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the completed task.</param>
        /// <param name="timestampUtc">When the task has completed in UTC.</param>
        /// <param name="totalCpuTime">The total processor time used during task execution.</param>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="totalCpuTime"/> is less then <see cref="TimeSpan.Zero"/>.</exception>
        public TaskCompletedEventArgs(Guid taskId, DateTime timestampUtc, TimeSpan totalCpuTime)
            : base(taskId, timestampUtc)
        {
            if (totalCpuTime < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("totalCpuTime", totalCpuTime, "Value must be positive.");
            }

            this.totalCpuTime = totalCpuTime;
        }

        /// <summary>
        /// Gets the total processor time used during task execution.
        /// </summary>
        /// <value>The total processor time used during task execution.</value>
        public TimeSpan TotalCpuTime
        {
            get { return this.totalCpuTime; }
        }
    }
}