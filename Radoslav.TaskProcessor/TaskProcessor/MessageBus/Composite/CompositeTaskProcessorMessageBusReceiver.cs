using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// A composite implementation of <see cref="ITaskProcessorMessageBusReceiver"/>.
    /// </summary>
    public sealed class CompositeTaskProcessorMessageBusReceiver : CompositeMessageBusReceiver<ITaskProcessorMessageBusReceiver>, ITaskProcessorMessageBusReceiver
    {
        #region ITaskProcessorMessageBusReceiver Members

        /// <inheritdoc />
        event EventHandler<TaskProcessorStateEventArgs> ITaskProcessorMessageBusReceiver.StateChanged
        {
            add
            {
                this.ForEach(mb => mb.StateChanged += value);
            }

            remove
            {
                this.ForEach(mb => mb.StateChanged -= value);
            }
        }

        event EventHandler<MasterModeChangeEventArgs> ITaskProcessorMessageBusReceiver.MasterModeChangeRequested
        {
            add
            {
                this.ForEach(mb => mb.MasterModeChangeRequested += value);
            }

            remove
            {
                this.ForEach(mb => mb.MasterModeChangeRequested -= value);
            }
        }

        event EventHandler<MasterModeChangeEventArgs> ITaskProcessorMessageBusReceiver.MasterModeChanged
        {
            add
            {
                this.ForEach(mb => mb.MasterModeChanged += value);
            }

            remove
            {
                this.ForEach(mb => mb.MasterModeChanged -= value);
            }
        }

        event EventHandler<TaskProcessorEventArgs> ITaskProcessorMessageBusReceiver.ConfigurationChanged
        {
            add
            {
                this.ForEach(mb => mb.ConfigurationChanged += value);
            }

            remove
            {
                this.ForEach(mb => mb.ConfigurationChanged -= value);
            }
        }

        /// <inheritdoc />
        event EventHandler<TaskProcessorEventArgs> ITaskProcessorMessageBusReceiver.StopRequested
        {
            add
            {
                this.ForEach(mb => mb.StopRequested += value);
            }

            remove
            {
                this.ForEach(mb => mb.StopRequested -= value);
            }
        }

        /// <inheritdoc />
        event EventHandler<PerformanceMonitoringEventArgs> ITaskProcessorMessageBusReceiver.PerformanceMonitoringRequested
        {
            add
            {
                this.ForEach(mb => mb.PerformanceMonitoringRequested += value);
            }

            remove
            {
                this.ForEach(mb => mb.PerformanceMonitoringRequested -= value);
            }
        }

        /// <inheritdoc />
        event EventHandler<TaskProcessorPerformanceEventArgs> ITaskProcessorMessageBusReceiver.PerformanceReportReceived
        {
            add
            {
                this.ForEach(mb => mb.PerformanceReportReceived += value);
            }

            remove
            {
                this.ForEach(mb => mb.PerformanceReportReceived -= value);
            }
        }

        #endregion ITaskProcessorMessageBusReceiver Members
    }
}