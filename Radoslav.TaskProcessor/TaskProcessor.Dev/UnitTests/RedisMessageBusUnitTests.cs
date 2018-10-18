using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisMessageBusUnitTests : MessageBusTestsBase
    {
        private new RedisTaskProcessorMessageBus MessageBus
        {
            get
            {
                return (RedisTaskProcessorMessageBus)base.MessageBus;
            }
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            Monitor.Enter(AssemblyUnitTests.RedisLockObject);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.MessageBus.Dispose();
            this.MessageBus.Provider.Dispose();

            Monitor.Exit(AssemblyUnitTests.RedisLockObject);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullProvider()
        {
            using (new RedisTaskProcessorMessageBus(null))
            {
            }
        }

        [TestMethod]
        public void SubscribeTimeoutInitial()
        {
            Assert.IsTrue(this.MessageBus.SubscribeTimeout > TimeSpan.Zero);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SubscribeTimeoutZero()
        {
            this.MessageBus.SubscribeTimeout = TimeSpan.Zero;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SubscribeForChannelsAfterDispose()
        {
            this.MessageBus.Dispose();

            this.MessageBus.SubscribeForChannels();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void UnsubscribeFromAllChannelsAfterDispose()
        {
            this.MessageBus.Dispose();

            this.MessageBus.UnsubscribeFromAllChannels();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void UnsubscribeFromAllChannelsExceptAfterDispose()
        {
            this.MessageBus.Dispose();

            this.MessageBus.UnsubscribeFromAllChannelsExcept();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void UnsubscribeFromChannelsAfterDispose()
        {
            this.MessageBus.Dispose();
            this.MessageBus.UnsubscribeFromChannels();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyMasterModeChangeRequestAfterDispose()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyMasterModeChangeRequest(Guid.Empty, false);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyMasterModeChangedAfterDispose()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyMasterModeChanged(Guid.Empty, false, MasterModeChangeReason.Explicit);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyTaskProcessorStateChangedAfterDispose()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyTaskProcessorStateChanged(Guid.Empty, TaskProcessorState.Inactive);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyTaskProcessorStopRequestedAfterDispose()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyTaskProcessorStopRequested(Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyTaskAssignedToProcessorAfterDispose()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyTaskAssigned(0, Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyTaskStartedAfterDispose()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyTaskStarted(0, Guid.Empty, DateTime.MinValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyTaskProgressAfterDispose()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyTaskProgress(0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyTaskCanceledAfterDispose()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyTaskCanceled(0, DateTime.MinValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyTaskCompletedAfterDispose()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyTaskCompleted(0, TaskStatus.Success, DateTime.MinValue, Guid.Empty, TimeSpan.FromMinutes(1), false);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyPerformanceMonitoringAfterDispose()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyPerformanceReportAfterDispose()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyPerformanceReport(new TaskProcessorPerformanceReport(Guid.Empty));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyConfigurationChangedDisposed1()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyConfigurationChanged();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NotifyConfigurationChangedDisposed2()
        {
            this.MessageBus.Dispose();
            this.MessageBus.NotifyConfigurationChanged(Guid.NewGuid());
        }

        [TestMethod]
        public override void RaiseTaskCompleted()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskCompletedAfterUnsubscribeFromAllChannelsExcept()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskCompletedAfterUnsubscribeFromChannels()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskProcessorStateChanged()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskProcessorStateChangedAfterUnsubscribeFromAllChannelsExcept()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskProcessorStateChangedAfterUnsubscribeFromChannels()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskProgress()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskProgressAfterUnsubscribeFromAllChannelsExcept()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskProgressAfterUnsubscribeFromChannels()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskRequested()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskRequestedAfterUnsubscribeFromAllChannelsExcept()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskRequestedAfterUnsubscribeFromChannels()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void ReportPerformance()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void ReportPerformanceAfterUnsubscribeFromAllChannelsExcept()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void ReportPerformanceAfterUnsubscribeFromChannels()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public void PopMasterCommandsQueue()
        {
            this.MessageBus.Provider.FlushAll();

            Guid taskProcessorId = Guid.NewGuid();

            ConfigurationChangedMasterCommand command1 = new ConfigurationChangedMasterCommand(taskProcessorId);

            this.MessageBus.MasterCommands.Add(command1);

            IUniqueMessage command2 = this.MessageBus.MasterCommands.PopFirst();

            Assert.IsNotNull(command2);
            Assert.IsInstanceOfType(command2, typeof(ConfigurationChangedMasterCommand));
            Assert.AreEqual(taskProcessorId, ((ConfigurationChangedMasterCommand)command2).TaskProcessorId);
        }

        [TestMethod]
        public void PopMasterCommandsQueueIfEmpty()
        {
            this.MessageBus.Provider.FlushAll();

            Assert.IsNull(this.MessageBus.MasterCommands.PopFirst());
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        protected override ITaskProcessorMessageBus CreateMessageBus()
        {
            return new RedisTaskProcessorMessageBus(new ServiceStackRedisProvider());
        }
    }
}