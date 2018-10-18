using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Event arguments for <see cref="ITaskProcessorMessageBusReceiver.PerformanceMonitoringRequested" />
    /// </summary>
    public sealed class PerformanceMonitoringEventArgs : EventArgs
    {
        private readonly TimeSpan refreshInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMonitoringEventArgs"/> class.
        /// </summary>
        /// <param name="refreshInterval">The time interval between two performance report messages sent by the task processor.</param>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="refreshInterval"/> is less than or equal to <see cref="TimeSpan.Zero"/>.</exception>
        public PerformanceMonitoringEventArgs(TimeSpan refreshInterval)
        {
            if (refreshInterval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("refreshInterval", refreshInterval, "Value must be positive.");
            }

            this.refreshInterval = refreshInterval;
        }

        /// <summary>
        /// Gets the time interval between two performance report messages sent by the task processor.
        /// </summary>
        /// <value>The time interval between two performance report messages sent by the task processor.</value>
        public TimeSpan RefreshInterval
        {
            get { return this.refreshInterval; }
        }
    }
}