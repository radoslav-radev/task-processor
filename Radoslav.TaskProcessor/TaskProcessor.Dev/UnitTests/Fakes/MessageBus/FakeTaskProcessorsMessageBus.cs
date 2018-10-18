using System;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskProcessorsMessageBus : FakeMessageBusBase, ITaskProcessorsMessageBus, ITaskProcessorMessageBusReceiver, ITaskProcessorMessageBusSender
    {
        internal FakeTaskProcessorsMessageBus(FakeMessageQueue masterCommands)
            : base(masterCommands)
        {
        }

        #region ITaskProcessorMessageBusReceiver Members

        public event EventHandler<TaskProcessorStateEventArgs> StateChanged;

        public event EventHandler<MasterModeChangeEventArgs> MasterModeChangeRequested;

        public event EventHandler<MasterModeChangeEventArgs> MasterModeChanged;

        public event EventHandler<TaskProcessorEventArgs> ConfigurationChanged;

        public event EventHandler<TaskProcessorEventArgs> StopRequested;

        public event EventHandler<PerformanceMonitoringEventArgs> PerformanceMonitoringRequested;

        public event EventHandler<TaskProcessorPerformanceEventArgs> PerformanceReportReceived;

        #region ITaskProcessorsMessageBus Members

        ITaskProcessorMessageBusSender ITaskProcessorsMessageBus.Sender
        {
            get { return this; }
        }

        ITaskProcessorMessageBusReceiver ITaskProcessorsMessageBus.Receiver
        {
            get { return this; }
        }

        #endregion ITaskProcessorsMessageBus Members

        #endregion ITaskProcessorMessageBusReceiver Members

        internal ITaskProcessorMessageBusSender Sender
        {
            get { return this; }
        }

        internal ITaskProcessorMessageBusReceiver Receiver
        {
            get { return this; }
        }

        #region ITaskProcessorMessageBusSender Members

        public void NotifyStateChanged(Guid taskProcessorId, TaskProcessorState state)
        {
            this.RecordMethodCall(taskProcessorId, state);

            if (state == TaskProcessorState.Active)
            {
                this.MasterCommands.Push(new TaskProcessorRegisteredMasterCommand(taskProcessorId));
            }

            if ((this.StateChanged != null) && this.SubscribedChannels.Contains(MessageBusChannel.TaskProcessorState))
            {
                this.StateChanged(this, new TaskProcessorStateEventArgs(taskProcessorId, state));
            }
        }

        public void NotifyMasterModeChangeRequest(Guid taskProcessorId, bool isMaster)
        {
            this.RecordMethodCall(taskProcessorId, isMaster);

            if ((this.MasterModeChangeRequested != null) && this.SubscribedChannels.Contains(MessageBusChannel.MasterModeChangeRequest))
            {
                this.MasterModeChangeRequested(this, new MasterModeChangeEventArgs(taskProcessorId, isMaster, MasterModeChangeReason.Explicit));
            }
        }

        public void NotifyMasterModeChanged(Guid taskProcessorId, bool isMaster, MasterModeChangeReason reason)
        {
            this.RecordMethodCall(taskProcessorId, isMaster, reason);

            MasterModeChangeEventArgs.ValidateArguments(isMaster, reason);

            if ((this.MasterModeChanged != null) && this.SubscribedChannels.Contains(MessageBusChannel.MasterModeChanged))
            {
                this.MasterModeChanged(this, new MasterModeChangeEventArgs(taskProcessorId, isMaster, reason));
            }
        }

        public void NotifyStopRequested(Guid taskProcessorId)
        {
            this.RecordMethodCall(taskProcessorId);

            if ((this.StopRequested != null) && this.SubscribedChannels.Contains(MessageBusChannel.StopTaskProcessor))
            {
                this.StopRequested(this, new TaskProcessorEventArgs(taskProcessorId));
            }
        }

        public void NotifyPerformanceMonitoring(TimeSpan refreshInterval)
        {
            this.RecordMethodCall(refreshInterval);

            if ((this.PerformanceMonitoringRequested != null) && this.SubscribedChannels.Contains(MessageBusChannel.PerformanceMonitoringRequest))
            {
                this.PerformanceMonitoringRequested(this, new PerformanceMonitoringEventArgs(refreshInterval));
            }
        }

        public void NotifyPerformanceReport(TaskProcessorPerformanceReport taskProcessorPerformanceInfo)
        {
            this.RecordMethodCall(taskProcessorPerformanceInfo);

            if ((this.PerformanceReportReceived != null) && this.SubscribedChannels.Contains(MessageBusChannel.PerformanceReport))
            {
                this.PerformanceReportReceived(this, new TaskProcessorPerformanceEventArgs(taskProcessorPerformanceInfo));
            }
        }

        public void NotifyConfigurationChanged(Guid taskProcessorId)
        {
            this.RecordMethodCall(taskProcessorId);

            this.MasterCommands.Push(new ConfigurationChangedMasterCommand(taskProcessorId));

            if (this.SubscribedChannels.Contains(MessageBusChannel.ConfigurationChanged))
            {
                this.ConfigurationChanged(this, new TaskProcessorEventArgs(taskProcessorId));
            }
        }

        #endregion ITaskProcessorMessageBusSender Members
    }
}