using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Basic functionality of a message bus receiver for task processors events.
    /// </summary>
    public interface ITaskProcessorMessageBusReceiver : IMessageBusReceiver
    {
        /// <summary>
        /// A task processor state has changed.
        /// </summary>
        event EventHandler<TaskProcessorStateEventArgs> StateChanged;

        /// <summary>
        /// A task processor has been requested to become master or slave.
        /// </summary>
        event EventHandler<MasterModeChangeEventArgs> MasterModeChangeRequested;

        /// <summary>
        /// A task processor has become a master or slave.
        /// </summary>
        event EventHandler<MasterModeChangeEventArgs> MasterModeChanged;

        /// <summary>
        /// A task processor configuration has been changed.
        /// </summary>
        event EventHandler<TaskProcessorEventArgs> ConfigurationChanged;

        /// <summary>
        /// A task processor has been requested to stop.
        /// </summary>
        event EventHandler<TaskProcessorEventArgs> StopRequested;

        /// <summary>
        /// A task processor has been asked to start performance monitoring.
        /// </summary>
        event EventHandler<PerformanceMonitoringEventArgs> PerformanceMonitoringRequested;

        /// <summary>
        /// A performance report from a task processor has been received.
        /// </summary>
        event EventHandler<TaskProcessorPerformanceEventArgs> PerformanceReportReceived;
    }
}