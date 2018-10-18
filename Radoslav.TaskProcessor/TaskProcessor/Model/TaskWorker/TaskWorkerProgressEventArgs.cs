using System;

namespace Radoslav.TaskProcessor.TaskWorker
{
    /// <summary>
    /// Event arguments for <see cref="ITaskWorker.ReportProgress"/> event.
    /// </summary>
    public sealed class TaskWorkerProgressEventArgs : EventArgs
    {
        private readonly double percentage;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskWorkerProgressEventArgs"/> class.
        /// </summary>
        /// <param name="percentage">The percent of completion.</param>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="percentage"/> is less than 0 or greater than 100.</exception>
        public TaskWorkerProgressEventArgs(double percentage)
        {
            if ((percentage < 0) || (percentage > 100))
            {
                throw new ArgumentOutOfRangeException("percentage", percentage, "Value must be between 0 and 100.");
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