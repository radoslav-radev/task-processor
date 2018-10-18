using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Event arguments for the <see cref="ITaskProcessorMessageBusReceiver.PerformanceReportReceived"/> event.
    /// </summary>
    public sealed class TaskProcessorPerformanceEventArgs : EventArgs
    {
        private readonly TaskProcessorPerformanceReport performanceReport;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProcessorPerformanceEventArgs"/> class.
        /// </summary>
        /// <param name="performanceReport">The performance report for the task processor.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="performanceReport"/> is null.</exception>
        public TaskProcessorPerformanceEventArgs(TaskProcessorPerformanceReport performanceReport)
        {
            if (performanceReport == null)
            {
                throw new ArgumentNullException("performanceReport");
            }

            this.performanceReport = performanceReport;
        }

        /// <summary>
        /// Gets the performance report for the task processor.
        /// </summary>
        /// <value>The performance report for the task processor.</value>
        public TaskProcessorPerformanceReport PerformanceInfo
        {
            get { return this.performanceReport; }
        }
    }
}