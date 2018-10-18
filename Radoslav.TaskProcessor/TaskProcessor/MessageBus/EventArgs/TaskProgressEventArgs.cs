using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Event arguments for <see cref="ITaskMessageBusReceiver.TaskProgress"/> event.
    /// </summary>
    public sealed class TaskProgressEventArgs : TaskEventArgs
    {
        private readonly double percentage;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProgressEventArgs"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the task that has progressed.</param>
        /// <param name="percentage">The percent of completion.</param>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="percentage"/> is less than 0 or greater than 100.</exception>
        public TaskProgressEventArgs(Guid taskId, double percentage)
            : base(taskId)
        {
            if ((percentage < 0) || (percentage > 100))
            {
                throw new ArgumentOutOfRangeException("percentage", percentage, "Value must not be negative.");
            }

            this.percentage = percentage;
        }

        /// <summary>
        /// Gets the percent of completion.
        /// </summary>
        /// <value>The percent of completion.</value>
        public double Percentage
        {
            get { return this.percentage; }
        }
    }
}