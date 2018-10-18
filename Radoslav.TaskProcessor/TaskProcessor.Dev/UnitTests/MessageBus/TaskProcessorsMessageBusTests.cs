using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class TaskProcessorsMessageBusTests : MessageBusBaseTests<ITaskProcessorMessageBusSender, ITaskProcessorMessageBusReceiver>
    {
        #region TaskProcessorStateChanged

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskProcessorStateChangedIfNotSubscribed()
        {
            Helpers.WaitForEvent<TaskProcessorStateEventArgs>(
                this.Timeout,
                handler => this.Receiver.StateChanged += handler,
                () => this.Sender.NotifyStateChanged(Guid.NewGuid(), TaskProcessorState.Active));
        }

        [TestMethod]
        public virtual void RaiseTaskProcessorStateChanged()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskProcessorState);

            TaskProcessorStateEventArgs args = Helpers.WaitForEvent<TaskProcessorStateEventArgs>(
                this.Timeout,
                handler => this.Receiver.StateChanged += handler,
                () => this.Sender.NotifyStateChanged(taskProcessorId, TaskProcessorState.Active));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
            Assert.AreEqual(TaskProcessorState.Active, args.TaskProcessorState);
        }

        [TestMethod]
        public virtual void RaiseTaskProcessorStateChangedAfterUnsubscribeFromChannels()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskProcessorState);
            this.Receiver.UnsubscribeFromChannels();

            TaskProcessorStateEventArgs args = Helpers.WaitForEvent<TaskProcessorStateEventArgs>(
                this.Timeout,
                handler => this.Receiver.StateChanged += handler,
                () => this.Sender.NotifyStateChanged(taskProcessorId, TaskProcessorState.Disposed));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
            Assert.AreEqual(TaskProcessorState.Disposed, args.TaskProcessorState);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskProcessorStateChangedAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskProcessorState);
            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.TaskProcessorState);

            Helpers.WaitForEvent<TaskProcessorStateEventArgs>(
                this.Timeout,
                handler => this.Receiver.StateChanged += handler,
                () => this.Sender.NotifyStateChanged(Guid.NewGuid(), TaskProcessorState.Active));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskProcessorStateChangedAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskProcessorState);
            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskProcessorStateEventArgs>(
                this.Timeout,
                handler => this.Receiver.StateChanged += handler,
                () => this.Sender.NotifyStateChanged(Guid.NewGuid(), TaskProcessorState.Active));
        }

        [TestMethod]
        public virtual void RaiseTaskProcessorStateChangedAfterUnsubscribeFromAllChannelsExcept()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskProcessorState);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.TaskProcessorState);

            TaskProcessorStateEventArgs args = Helpers.WaitForEvent<TaskProcessorStateEventArgs>(
                this.Timeout,
                handler => this.Receiver.StateChanged += handler,
                () => this.Sender.NotifyStateChanged(taskProcessorId, TaskProcessorState.Stopping));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
            Assert.AreEqual(TaskProcessorState.Stopping, args.TaskProcessorState);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskProcessorStateChangedAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskProcessorState);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<TaskProcessorStateEventArgs>(
                this.Timeout,
                handler => this.Receiver.StateChanged += handler,
                () => this.Sender.NotifyStateChanged(Guid.NewGuid(), TaskProcessorState.Active));
        }

        #endregion TaskProcessorStateChanged

        #region MasterModeChangeRequest

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseMasterModeChangeRequestedIfNotSubscribed()
        {
            Helpers.WaitForEvent<MasterModeChangeEventArgs>(
               this.Timeout,
               handler => this.Receiver.MasterModeChangeRequested += handler,
               () => this.Sender.NotifyMasterModeChangeRequest(Guid.Empty, false));
        }

        [TestMethod]
        public virtual void RaiseMasterModeChangeRequested()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.MasterModeChangeRequest);

            MasterModeChangeEventArgs args = Helpers.WaitForEvent<MasterModeChangeEventArgs>(
                this.Timeout,
                handler => this.Receiver.MasterModeChangeRequested += handler,
                () => this.Sender.NotifyMasterModeChangeRequest(taskProcessorId, true));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
            Assert.IsTrue(args.IsMaster);
            Assert.AreEqual(MasterModeChangeReason.Explicit, args.Reason);
        }

        [TestMethod]
        public virtual void RaiseMasterModeChangeRequestedAfterUnsubscribeFromChannels()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.MasterModeChangeRequest);
            this.Receiver.UnsubscribeFromChannels();

            MasterModeChangeEventArgs args = Helpers.WaitForEvent<MasterModeChangeEventArgs>(
                this.Timeout,
                handler => this.Receiver.MasterModeChangeRequested += handler,
                () => this.Sender.NotifyMasterModeChangeRequest(taskProcessorId, false));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
            Assert.IsFalse(args.IsMaster);
            Assert.AreEqual(MasterModeChangeReason.Explicit, args.Reason);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseMasterModeChangeRequestedAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.MasterModeChangeRequest);
            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.MasterModeChangeRequest);

            Helpers.WaitForEvent<MasterModeChangeEventArgs>(
                this.Timeout,
                handler => this.Receiver.MasterModeChangeRequested += handler,
               () => this.Sender.NotifyMasterModeChangeRequest(Guid.Empty, false));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseMasterModeChangeRequestedAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.MasterModeChangeRequest);
            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<MasterModeChangeEventArgs>(
                this.Timeout,
                handler => this.Receiver.MasterModeChangeRequested += handler,
                () => this.Sender.NotifyMasterModeChangeRequest(Guid.Empty, false));
        }

        [TestMethod]
        public virtual void RaiseMasterModeChangeRequestedAfterUnsubscribeFromAllChannelsExcept()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.MasterModeChangeRequest);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.MasterModeChangeRequest);

            MasterModeChangeEventArgs args = Helpers.WaitForEvent<MasterModeChangeEventArgs>(
                this.Timeout,
                handler => this.Receiver.MasterModeChangeRequested += handler,
                () => this.Sender.NotifyMasterModeChangeRequest(taskProcessorId, true));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
            Assert.IsTrue(args.IsMaster);
            Assert.AreEqual(MasterModeChangeReason.Explicit, args.Reason);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseMasterModeChangeRequestedAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.MasterModeChangeRequest);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<MasterModeChangeEventArgs>(
                this.Timeout,
                handler => this.Receiver.MasterModeChangeRequested += handler,
                () => this.Sender.NotifyMasterModeChangeRequest(Guid.Empty, false));
        }

        #endregion MasterModeChangeRequest

        #region MasterModeChanged

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NotifyMasterModeChangedNone()
        {
            this.Sender.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.None);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NotifyMasterModeChangedStartSlave()
        {
            this.Sender.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.Start);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseMasterModeChangedIfNotSubscribed()
        {
            Helpers.WaitForEvent<MasterModeChangeEventArgs>(
               this.Timeout,
               handler => this.Receiver.MasterModeChanged += handler,
               () => this.Sender.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.Explicit));
        }

        [TestMethod]
        public virtual void RaiseMasterModeChanged()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.MasterModeChanged);

            MasterModeChangeEventArgs args = Helpers.WaitForEvent<MasterModeChangeEventArgs>(
                this.Timeout,
                handler => this.Receiver.MasterModeChanged += handler,
                () => this.Sender.NotifyMasterModeChanged(taskProcessorId, true, MasterModeChangeReason.Start));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
            Assert.IsTrue(args.IsMaster);
            Assert.AreEqual(MasterModeChangeReason.Start, args.Reason);
        }

        [TestMethod]
        public virtual void RaiseMasterModeChangedAfterUnsubscribeFromChannels()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.MasterModeChanged);
            this.Receiver.UnsubscribeFromChannels();

            MasterModeChangeEventArgs args = Helpers.WaitForEvent<MasterModeChangeEventArgs>(
                this.Timeout,
                handler => this.Receiver.MasterModeChanged += handler,
                () => this.Sender.NotifyMasterModeChanged(taskProcessorId, false, MasterModeChangeReason.Stop));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
            Assert.IsFalse(args.IsMaster);
            Assert.AreEqual(MasterModeChangeReason.Stop, args.Reason);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseMasterModeChangedAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.MasterModeChanged);
            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.MasterModeChanged);

            Helpers.WaitForEvent<MasterModeChangeEventArgs>(
                this.Timeout,
                handler => this.Receiver.MasterModeChanged += handler,
               () => this.Sender.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.Explicit));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseMasterModeChangedAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.MasterModeChanged);
            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<MasterModeChangeEventArgs>(
                this.Timeout,
                handler => this.Receiver.MasterModeChanged += handler,
                () => this.Sender.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.Heartbeat));
        }

        [TestMethod]
        public virtual void RaiseMasterModeChangedAfterUnsubscribeFromAllChannelsExcept()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.MasterModeChanged);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.MasterModeChanged);

            MasterModeChangeEventArgs args = Helpers.WaitForEvent<MasterModeChangeEventArgs>(
                this.Timeout,
                handler => this.Receiver.MasterModeChanged += handler,
                () => this.Sender.NotifyMasterModeChanged(taskProcessorId, true, MasterModeChangeReason.Heartbeat));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
            Assert.IsTrue(args.IsMaster);
            Assert.AreEqual(MasterModeChangeReason.Heartbeat, args.Reason);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseMasterModeChangedAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.MasterModeChanged);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<MasterModeChangeEventArgs>(
                this.Timeout,
                handler => this.Receiver.MasterModeChangeRequested += handler,
                () => this.Sender.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.Explicit));
        }

        #endregion MasterModeChanged

        #region TaskProcessorStopRequested

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskProcessorStopRequestedIfNotSubscribed()
        {
            Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.StopRequested += handler,
                () => this.Sender.NotifyStopRequested(Guid.NewGuid()));
        }

        [TestMethod]
        public void RaiseTaskProcessorStopRequested()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.StopTaskProcessor);

            TaskProcessorEventArgs args = Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.StopRequested += handler,
                () => this.Sender.NotifyStopRequested(taskProcessorId));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
        }

        [TestMethod]
        public void RaiseTaskProcessorStopRequestedAfterUnsubscribeFromChannels()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.StopTaskProcessor);
            this.Receiver.UnsubscribeFromChannels();

            TaskProcessorEventArgs args = Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.StopRequested += handler,
                () => this.Sender.NotifyStopRequested(taskProcessorId));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskProcessorStopRequestedAfterUnsubscribeFromChannels()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.StopTaskProcessor);
            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.StopTaskProcessor);

            Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.StopRequested += handler,
                () => this.Sender.NotifyStopRequested(taskProcessorId));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskProcessorStopRequestedAfterUnsubscribeFromAllChannels()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.StopTaskProcessor);
            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.StopRequested += handler,
                () => this.Sender.NotifyStopRequested(taskProcessorId));
        }

        [TestMethod]
        public void RaiseTaskProcessorStopRequestedAfterUnsubscribeFromChannelsExcept()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.StopTaskProcessor);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.StopTaskProcessor);

            TaskProcessorEventArgs args = Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.StopRequested += handler,
                () => this.Sender.NotifyStopRequested(taskProcessorId));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskProcessorStopRequestedAfterUnsubscribeFromChannelsExcept()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.StopTaskProcessor);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.StopRequested += handler,
                () => this.Sender.NotifyStopRequested(taskProcessorId));
        }

        #endregion TaskProcessorStopRequested

        #region MonitoringStarted

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RaiseMonitoringStartedRefreshIntervalNegative()
        {
            this.Sender.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(-1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RaiseMonitoringStartedRefreshIntervalZero()
        {
            this.Sender.NotifyPerformanceMonitoring(TimeSpan.Zero);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseMonitoringStartedIfNotSubscribed()
        {
            Helpers.WaitForEvent<PerformanceMonitoringEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceMonitoringRequested += handler,
                () => this.Sender.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1)));
        }

        [TestMethod]
        public void RaisePerformanceMonitoring()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.PerformanceMonitoringRequest);

            PerformanceMonitoringEventArgs args = Helpers.WaitForEvent<PerformanceMonitoringEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceMonitoringRequested += handler,
                () => this.Sender.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1.5)));

            Assert.AreEqual(TimeSpan.FromSeconds(1.5), args.RefreshInterval);
        }

        [TestMethod]
        public void RaisePerformanceMonitoringAfterUnsubscribedFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.PerformanceMonitoringRequest);
            this.Receiver.UnsubscribeFromChannels();

            PerformanceMonitoringEventArgs args = Helpers.WaitForEvent<PerformanceMonitoringEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceMonitoringRequested += handler,
                () => this.Sender.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(3.5)));

            Assert.AreEqual(TimeSpan.FromSeconds(3.5), args.RefreshInterval);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaisePerformanceMonitoringAfterUnsubscribedFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.PerformanceMonitoringRequest);

            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.PerformanceMonitoringRequest);

            Helpers.WaitForEvent<PerformanceMonitoringEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceMonitoringRequested += handler,
                () => this.Sender.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1.5)));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaisePerformanceMonitoringAfterUnsubscribedFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.PerformanceMonitoringRequest);
            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<PerformanceMonitoringEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceMonitoringRequested += handler,
                () => this.Sender.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1.5)));
        }

        [TestMethod]
        public void RaisePerformanceMonitoringAfterUnsubscribedFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.PerformanceMonitoringRequest);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.PerformanceMonitoringRequest);

            PerformanceMonitoringEventArgs args = Helpers.WaitForEvent<PerformanceMonitoringEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceMonitoringRequested += handler,
                () => this.Sender.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(2.5)));

            Assert.AreEqual(TimeSpan.FromSeconds(2.5), args.RefreshInterval);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaisePerformanceMonitoringAfterUnsubscribedFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.PerformanceMonitoringRequest);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<PerformanceMonitoringEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceMonitoringRequested += handler,
                () => this.Sender.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1.5)));
        }

        #endregion MonitoringStarted

        #region ReportPerformance

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotReportPerformanceIfNotSubscribed()
        {
            Helpers.WaitForEvent<TaskProcessorPerformanceEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceReportReceived += handler,
                () => this.Sender.NotifyPerformanceReport(new TaskProcessorPerformanceReport(Guid.NewGuid())));
        }

        [TestMethod]
        public virtual void ReportPerformance()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.PerformanceReport);

            TaskProcessorPerformanceReport performance = new TaskProcessorPerformanceReport(Guid.NewGuid())
            {
                CpuPercent = 23,
                RamPercent = 31
            };

            performance.TasksPerformance.Add(new TaskPerformanceReport(Guid.NewGuid())
            {
                CpuPercent = 45,
                RamPercent = 46
            });

            performance.TasksPerformance.Add(new TaskPerformanceReport(Guid.NewGuid())
            {
                CpuPercent = 76,
                RamPercent = 84
            });

            TaskProcessorPerformanceEventArgs args = Helpers.WaitForEvent<TaskProcessorPerformanceEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceReportReceived += handler,
                () => this.Sender.NotifyPerformanceReport(performance));

            UnitTestHelpers.AssertEqualByPublicScalarProperties(performance, args.PerformanceInfo);

            Assert.IsTrue(performance.TasksPerformance.IsEquivalentTo(
                args.PerformanceInfo.TasksPerformance,
                (a, b) => UnitTestHelpers.AreEqualByPublicScalarProperties(a, b)));
        }

        [TestMethod]
        public virtual void ReportPerformanceAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.PerformanceReport);
            this.Receiver.UnsubscribeFromChannels();

            Helpers.WaitForEvent<TaskProcessorPerformanceEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceReportReceived += handler,
                () => this.Sender.NotifyPerformanceReport(new TaskProcessorPerformanceReport(Guid.NewGuid())));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotReportPerformanceAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.PerformanceReport);

            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.PerformanceReport);

            Helpers.WaitForEvent<TaskProcessorPerformanceEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceReportReceived += handler,
                () => this.Sender.NotifyPerformanceReport(new TaskProcessorPerformanceReport(Guid.NewGuid())));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotReportPerformanceAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.PerformanceReport);
            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskProcessorPerformanceEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceReportReceived += handler,
                () => this.Sender.NotifyPerformanceReport(new TaskProcessorPerformanceReport(Guid.NewGuid())));
        }

        [TestMethod]
        public virtual void ReportPerformanceAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.PerformanceReport);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.PerformanceReport);

            Helpers.WaitForEvent<TaskProcessorPerformanceEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceReportReceived += handler,
                () => this.Sender.NotifyPerformanceReport(new TaskProcessorPerformanceReport(Guid.NewGuid())));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotReportPerformanceAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.PerformanceReport);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<TaskProcessorPerformanceEventArgs>(
                this.Timeout,
                handler => this.Receiver.PerformanceReportReceived += handler,
                () => this.Sender.NotifyPerformanceReport(new TaskProcessorPerformanceReport(Guid.NewGuid())));
        }

        #endregion ReportPerformance

        #region Configuration Changed

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseConfigurationChangedIfNotSubscribed()
        {
            Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.ConfigurationChanged += handler,
                () => this.Sender.NotifyConfigurationChanged(Guid.Empty));
        }

        [TestMethod]
        public void RaiseConfigurationChanged()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.ConfigurationChanged);

            Guid taskProcessorId = Guid.NewGuid();

            TaskProcessorEventArgs args = Helpers.WaitForEvent<TaskProcessorEventArgs>(
              this.Timeout,
              handler => this.Receiver.ConfigurationChanged += handler,
              () => this.Sender.NotifyConfigurationChanged(taskProcessorId));

            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
        }

        [TestMethod]
        public void RaiseConfigurationChangedAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.ConfigurationChanged);
            this.Receiver.UnsubscribeFromChannels();

            Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.ConfigurationChanged += handler,
                () => this.Sender.NotifyConfigurationChanged(Guid.Empty));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseConfigurationChangedAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.ConfigurationChanged);
            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.ConfigurationChanged);

            Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.ConfigurationChanged += handler,
                () => this.Sender.NotifyConfigurationChanged(Guid.Empty));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseConfigurationChangedAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.ConfigurationChanged);
            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.ConfigurationChanged += handler,
                () => this.Sender.NotifyConfigurationChanged(Guid.Empty));
        }

        [TestMethod]
        public void RaiseConfigurationChangedAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.ConfigurationChanged);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.ConfigurationChanged);

            Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.ConfigurationChanged += handler,
                () => this.Sender.NotifyConfigurationChanged(Guid.Empty));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseConfigurationChangedAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.ConfigurationChanged);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<TaskProcessorEventArgs>(
                this.Timeout,
                handler => this.Receiver.ConfigurationChanged += handler,
                () => this.Sender.NotifyConfigurationChanged(Guid.Empty));
        }

        #endregion Configuration Changed

        #region Master Commands

        [TestMethod]
        public void AddTaskProcessorRegisteredMasterCommandIfStateIsActive()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Sender.NotifyStateChanged(taskProcessorId, TaskProcessorState.Active);

            Thread.Sleep(this.Timeout);

            Assert.IsTrue(this.MasterCommands
                .OfType<TaskProcessorRegisteredMasterCommand>()
                .Any(c => c.TaskProcessorId == taskProcessorId));
        }

        [TestMethod]
        public void DoNotAddTaskProcessorRegisteredMasterCommandIfStateIsInactive()
        {
            this.DoNotAddTaskProcessorRegisteredMasterCommandIfStateIsNotActive(TaskProcessorState.Inactive);
        }

        [TestMethod]
        public void DoNotAddTaskProcessorRegisteredMasterCommandIfStateIsStopping()
        {
            this.DoNotAddTaskProcessorRegisteredMasterCommandIfStateIsNotActive(TaskProcessorState.Stopping);
        }

        [TestMethod]
        public void AddConfigurationChangedMasterCommand()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Sender.NotifyConfigurationChanged(taskProcessorId);

            Assert.IsTrue(this.MasterCommands
                .OfType<ConfigurationChangedMasterCommand>()
                .Any(c => c.TaskProcessorId == taskProcessorId));
        }

        private void DoNotAddTaskProcessorRegisteredMasterCommandIfStateIsNotActive(TaskProcessorState state)
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Sender.NotifyStateChanged(taskProcessorId, state);

            Thread.Sleep(this.Timeout);

            Assert.IsFalse(this.MasterCommands.OfType<TaskProcessorRegisteredMasterCommand>().Any(c => c.TaskProcessorId == taskProcessorId));
        }

        #endregion Master Commands
    }
}