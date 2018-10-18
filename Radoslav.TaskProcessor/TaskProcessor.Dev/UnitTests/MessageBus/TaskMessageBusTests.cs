using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.MessageBus;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class TaskMessageBusTests : MessageBusBaseTests<ITaskMessageBusSender, ITaskMessageBusReceiver>
    {
        #region TaskSubmitted

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskSubmittedIfNotSubscribed()
        {
            Helpers.WaitForEvent<TaskEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskSubmitted += handler,
                () => this.Sender.NotifyTaskSubmitted(Guid.NewGuid(), DateTime.UtcNow, false));
        }

        [TestMethod]
        public virtual void RaiseTaskSubmitted()
        {
            Guid taskId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskSubmitted);

            TaskEventArgs args = Helpers.WaitForEvent<TaskEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskSubmitted += handler,
                () => this.Sender.NotifyTaskSubmitted(taskId, DateTime.UtcNow, false));

            Assert.AreEqual(taskId, args.TaskId);
        }

        [TestMethod]
        public virtual void RaiseTaskSubmittedAfterUnsubscribeFromChannels()
        {
            Guid taskId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskSubmitted);
            this.Receiver.UnsubscribeFromChannels();

            TaskEventArgs args = Helpers.WaitForEvent<TaskEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskSubmitted += handler,
                () => this.Sender.NotifyTaskSubmitted(taskId, DateTime.UtcNow, false));

            Assert.AreEqual(taskId, args.TaskId);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskSubmittedAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskSubmitted);
            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.TaskSubmitted);

            Helpers.WaitForEvent<TaskEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskSubmitted += handler,
                () => this.Sender.NotifyTaskSubmitted(Guid.NewGuid(), DateTime.UtcNow, false));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskSubmittedAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskSubmitted);

            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskSubmitted += handler,
                () => this.Sender.NotifyTaskSubmitted(Guid.NewGuid(), DateTime.UtcNow, false));
        }

        [TestMethod]
        public virtual void RaiseTaskSubmittedAfterUnsubscribeFromAllChannelsExcept()
        {
            Guid taskId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskSubmitted);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.TaskSubmitted);

            TaskEventArgs args = Helpers.WaitForEvent<TaskEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskSubmitted += handler,
                () => this.Sender.NotifyTaskSubmitted(taskId, DateTime.UtcNow, false));

            Assert.AreEqual(taskId, args.TaskId);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskSubmittedAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskSubmitted);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<TaskEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskSubmitted += handler,
                () => this.Sender.NotifyTaskSubmitted(Guid.NewGuid(), DateTime.UtcNow, false));
        }

        #endregion TaskSubmitted

        #region TaskAssignedToProcessor

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskAssignedToProcessorIfNotSubscribed()
        {
            Helpers.WaitForEvent<TaskAssignedEventArgs>(
               this.Timeout,
               handler => this.Receiver.TaskAssigned += handler,
               () => this.Sender.NotifyTaskAssigned(Guid.Empty, Guid.NewGuid()));
        }

        [TestMethod]
        public void RaiseTaskAssignedToProcessor()
        {
            Guid taskProcessorId = Guid.NewGuid();

            Guid taskId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskAssigned);

            TaskAssignedEventArgs args = Helpers.WaitForEvent<TaskAssignedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskAssigned += handler,
                () => this.Sender.NotifyTaskAssigned(taskId, taskProcessorId));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
        }

        [TestMethod]
        public void RaiseTaskAssignedToProcessorAfterUnsubscribeFromChannels()
        {
            Guid taskProcessorId = Guid.NewGuid();

            Guid taskId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskAssigned);
            this.Receiver.UnsubscribeFromChannels();

            TaskAssignedEventArgs args = Helpers.WaitForEvent<TaskAssignedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskAssigned += handler,
                () => this.Sender.NotifyTaskAssigned(taskId, taskProcessorId));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskAssignedToProcessorAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskAssigned);

            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.TaskAssigned);

            Helpers.WaitForEvent<TaskAssignedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskAssigned += handler,
                () => this.Sender.NotifyTaskAssigned(Guid.NewGuid(), Guid.NewGuid()));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskAssignedToProcessorAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskAssigned);

            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskAssignedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskAssigned += handler,
                () => this.Sender.NotifyTaskAssigned(Guid.NewGuid(), Guid.NewGuid()));
        }

        [TestMethod]
        public void RaiseTaskAssignedToProcessorAfterUnsubscribeFromAllChannelsExcept()
        {
            Guid taskProcessorId = Guid.NewGuid();

            Guid taskId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskAssigned);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.TaskAssigned);

            TaskAssignedEventArgs args = Helpers.WaitForEvent<TaskAssignedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskAssigned += handler,
                () => this.Sender.NotifyTaskAssigned(taskId, taskProcessorId));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskAssignedToProcessorAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskAssigned);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskAssignedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskAssigned += handler,
                () => this.Sender.NotifyTaskAssigned(Guid.NewGuid(), Guid.NewGuid()));
        }

        #endregion TaskAssignedToProcessor

        #region TaskStarted

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskStartedIfNotSubscribed()
        {
            Helpers.WaitForEvent<TaskStartedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskStarted += handler,
                () => this.Sender.NotifyTaskStarted(Guid.Empty, Guid.NewGuid(), DateTime.UtcNow));
        }

        [TestMethod]
        public void RaiseTaskStarted()
        {
            Guid taskId = Guid.NewGuid();

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskStarted);

            TaskStartedEventArgs args = Helpers.WaitForEvent<TaskStartedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskStarted += handler,
                () => this.Sender.NotifyTaskStarted(taskId, taskProcessorId, timestampUtc));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
            Assert.AreEqual(timestampUtc, args.TimestampUtc);
        }

        [TestMethod]
        public void RaiseTaskStartedAfterUnsubscribedFromChannels()
        {
            Guid taskId = Guid.NewGuid();

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskStarted);
            this.Receiver.UnsubscribeFromChannels();

            TaskStartedEventArgs args = Helpers.WaitForEvent<TaskStartedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskStarted += handler,
                () => this.Sender.NotifyTaskStarted(taskId, taskProcessorId, timestampUtc));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
            Assert.AreEqual(timestampUtc, args.TimestampUtc);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskStartedAfterUnsubscribedFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskStarted);

            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.TaskStarted);

            Helpers.WaitForEvent<TaskStartedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskStarted += handler,
                () => this.Sender.NotifyTaskStarted(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskStartedAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskStarted);

            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskStartedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskStarted += handler,
                () => this.Sender.NotifyTaskStarted(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow));
        }

        [TestMethod]
        public void RaiseTaskStartedAfterUnsubscribeFromAllChannelsExcept()
        {
            Guid taskId = Guid.NewGuid();

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskStarted);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.TaskStarted);

            TaskStartedEventArgs args = Helpers.WaitForEvent<TaskStartedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskStarted += handler,
                () => this.Sender.NotifyTaskStarted(taskId, taskProcessorId, timestampUtc));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(taskProcessorId, args.TaskProcessorId);
            Assert.AreEqual(timestampUtc, args.TimestampUtc);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskStartedAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskStarted);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskStartedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskStarted += handler,
                () => this.Sender.NotifyTaskStarted(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow));
        }

        #endregion TaskStarted

        #region TaskProgress

        [TestMethod]
        public void NotifyTaskProgress0()
        {
            this.Sender.NotifyTaskProgress(Guid.Empty, 0);
        }

        [TestMethod]
        public void NotifyTaskProgress100()
        {
            this.Sender.NotifyTaskProgress(Guid.Empty, 100);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskProgressIfNotSubscribed()
        {
            Helpers.WaitForEvent<TaskProgressEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskProgress += handler,
                () => this.Sender.NotifyTaskProgress(Guid.NewGuid(), 50));
        }

        [TestMethod]
        public virtual void RaiseTaskProgress()
        {
            Guid taskId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskProgress);

            TaskProgressEventArgs args = Helpers.WaitForEvent<TaskProgressEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskProgress += handler,
                () => this.Sender.NotifyTaskProgress(taskId, 11));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(11, args.Percentage);
        }

        [TestMethod]
        public virtual void RaiseTaskProgressAfterUnsubscribeFromChannels()
        {
            Guid taskId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskProgress);
            this.Receiver.UnsubscribeFromChannels();

            TaskProgressEventArgs args = Helpers.WaitForEvent<TaskProgressEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskProgress += handler,
                () => this.Sender.NotifyTaskProgress(taskId, 11));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(11, args.Percentage);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskProgressAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskProgress);

            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.TaskProgress);

            Helpers.WaitForEvent<TaskProgressEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskProgress += handler,
                () => this.Sender.NotifyTaskProgress(Guid.NewGuid(), 11));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskProgressAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskProgress);
            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskProgressEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskProgress += handler,
                () => this.Sender.NotifyTaskProgress(Guid.NewGuid(), 11));
        }

        [TestMethod]
        public virtual void RaiseTaskProgressAfterUnsubscribeFromAllChannelsExcept()
        {
            Guid taskId = Guid.NewGuid();

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskProgress);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.TaskProgress);

            TaskProgressEventArgs args = Helpers.WaitForEvent<TaskProgressEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskProgress += handler,
                () => this.Sender.NotifyTaskProgress(taskId, 11));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(11, args.Percentage);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskProgressAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskProgress);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<TaskProgressEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskProgress += handler,
                () => this.Sender.NotifyTaskProgress(Guid.NewGuid(), 11));
        }

        #endregion TaskProgress

        #region TaskCancelRequest

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCancelRequestIfNotSubscribed()
        {
            Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelRequested += handler,
                () => this.Sender.NotifyTaskCancelRequest(Guid.NewGuid(), DateTime.UtcNow));
        }

        [TestMethod]
        public void RaiseTaskCancelRequest()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelRequest);

            TaskEventEventArgs args = Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelRequested += handler,
                () => this.Sender.NotifyTaskCancelRequest(taskId, timestampUtc));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(timestampUtc, args.TimestampUtc);
        }

        [TestMethod]
        public void RaiseTaskCancelRequestAfterUnsubscribeFromChannels()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelRequest);
            this.Receiver.UnsubscribeFromChannels();

            TaskEventEventArgs args = Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelRequested += handler,
                () => this.Sender.NotifyTaskCancelRequest(taskId, timestampUtc));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(timestampUtc, args.TimestampUtc);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCancelRequestAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelRequest);
            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.TaskCancelRequest);

            Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelRequested += handler,
                () => this.Sender.NotifyTaskCancelRequest(Guid.NewGuid(), DateTime.UtcNow));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCancelRequestAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelRequest);

            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelRequested += handler,
                () => this.Sender.NotifyTaskCancelRequest(Guid.NewGuid(), DateTime.UtcNow));
        }

        [TestMethod]
        public void RaiseTaskCancelRequestAfterUnsubscribeFromAllChannelsExcept()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestampUtc = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelRequest);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.TaskCancelRequest);

            TaskEventEventArgs args = Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelRequested += handler,
                () => this.Sender.NotifyTaskCancelRequest(taskId, timestampUtc));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(timestampUtc, args.TimestampUtc);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCancelRequestAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelRequest);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelRequested += handler,
                () => this.Sender.NotifyTaskCancelRequest(Guid.NewGuid(), DateTime.UtcNow));
        }

        #endregion TaskCancelRequest

        #region TaskCancelCompleted

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCancelCompletedIfNotSubscribed()
        {
            Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelCompleted += handler,
                () => this.Sender.NotifyTaskCancelCompleted(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false));
        }

        [TestMethod]
        public virtual void RaiseTaskCancelCompleted()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelCompleted);

            TaskEventEventArgs args = Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelCompleted += handler,
                () => this.Sender.NotifyTaskCancelCompleted(taskId, timestamp, Guid.NewGuid(), false));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(timestamp, args.TimestampUtc);
        }

        [TestMethod]
        public virtual void RaiseTaskCancelCompletedAfterUnsubscribeFromChannels()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelCompleted);
            this.Receiver.UnsubscribeFromChannels();

            TaskEventEventArgs args = Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelCompleted += handler,
                () => this.Sender.NotifyTaskCancelCompleted(taskId, timestamp, Guid.NewGuid(), false));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(timestamp, args.TimestampUtc);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCancelCompletedAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelCompleted);
            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.TaskCancelCompleted);

            Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelCompleted += handler,
                () => this.Sender.NotifyTaskCancelCompleted(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCancelCompletedAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelCompleted);
            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelCompleted += handler,
                () => this.Sender.NotifyTaskCancelCompleted(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false));
        }

        [TestMethod]
        public virtual void RaiseTaskCancelCompletedAfterUnsubscribeFromAllChannelsExcept()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelCompleted);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.TaskCancelCompleted);

            TaskEventEventArgs args = Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelCompleted += handler,
                () => this.Sender.NotifyTaskCancelCompleted(taskId, timestamp, Guid.NewGuid(), false));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(timestamp, args.TimestampUtc);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCancelCompletedAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelCompleted);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCancelCompleted += handler,
                () => this.Sender.NotifyTaskCancelCompleted(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false));
        }

        #endregion TaskCancelCompleted

        #region TaskFailed

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NotifyTaskFailedNullError()
        {
            this.Sender.NotifyTaskFailed(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NotifyTaskFailedEmptyError()
        {
            this.Sender.NotifyTaskFailed(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskFailedIfNotSubscribed()
        {
            Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskFailed += handler,
                () => this.Sender.NotifyTaskFailed(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false, "Error"));
        }

        [TestMethod]
        public virtual void RaiseTaskFailed()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskFailed);

            TaskEventEventArgs args = Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskFailed += handler,
                () => this.Sender.NotifyTaskFailed(taskId, timestamp, Guid.NewGuid(), false, "Hello Error"));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(timestamp, args.TimestampUtc);
        }

        [TestMethod]
        public virtual void RaiseTaskFailedAfterUnsubscribeFromChannels()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskFailed);
            this.Receiver.UnsubscribeFromChannels();

            TaskEventEventArgs args = Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskFailed += handler,
                () => this.Sender.NotifyTaskFailed(taskId, timestamp, Guid.NewGuid(), false, "Hello Error"));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(timestamp, args.TimestampUtc);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskFailedAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskFailed);
            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.TaskFailed);

            Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskFailed += handler,
                () => this.Sender.NotifyTaskFailed(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false, "Hello Error"));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskFailedAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskFailed);
            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskFailed += handler,
                () => this.Sender.NotifyTaskFailed(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false, "Hello Error"));
        }

        [TestMethod]
        public virtual void RaiseTaskFailedAfterUnsubscribeFromAllChannelsExcept()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskFailed);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.TaskFailed);

            TaskEventEventArgs args = Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskFailed += handler,
                () => this.Sender.NotifyTaskFailed(taskId, timestamp, Guid.NewGuid(), false, "Hello Error"));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(timestamp, args.TimestampUtc);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskFailedAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskFailed);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<TaskEventEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskFailed += handler,
                () => this.Sender.NotifyTaskFailed(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false, "Hello Error"));
        }

        #endregion TaskFailed

        #region TaskCompleted

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCompletedIfNotSubscribed()
        {
            Helpers.WaitForEvent<TaskCompletedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCompleted += handler,
                () => this.Sender.NotifyTaskCompleted(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false, TimeSpan.FromSeconds(1)));
        }

        [TestMethod]
        public virtual void RaiseTaskCompleted()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCompleted);

            TaskCompletedEventArgs args = Helpers.WaitForEvent<TaskCompletedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCompleted += handler,
                () => this.Sender.NotifyTaskCompleted(taskId, timestamp, Guid.NewGuid(), false, TimeSpan.FromSeconds(1)));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(timestamp, args.TimestampUtc);
            Assert.AreEqual(TimeSpan.FromSeconds(1), args.TotalCpuTime);
        }

        [TestMethod]
        public virtual void RaiseTaskCompletedAfterUnsubscribeFromChannels()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCompleted);
            this.Receiver.UnsubscribeFromChannels();

            TaskCompletedEventArgs args = Helpers.WaitForEvent<TaskCompletedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCompleted += handler,
                () => this.Sender.NotifyTaskCompleted(taskId, timestamp, Guid.NewGuid(), false, TimeSpan.FromSeconds(1)));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(timestamp, args.TimestampUtc);
            Assert.AreEqual(TimeSpan.FromSeconds(1), args.TotalCpuTime);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCompletedAfterUnsubscribeFromChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCompleted);

            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.TaskCompleted);

            Helpers.WaitForEvent<TaskCompletedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCompleted += handler,
                () => this.Sender.NotifyTaskCompleted(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false, TimeSpan.FromSeconds(1)));
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCompletedAfterUnsubscribeFromAllChannels()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCompleted);
            this.Receiver.UnsubscribeFromAllChannels();

            Helpers.WaitForEvent<TaskCompletedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCompleted += handler,
                () => this.Sender.NotifyTaskCompleted(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false, TimeSpan.FromSeconds(1)));
        }

        [TestMethod]
        public virtual void RaiseTaskCompletedAfterUnsubscribeFromAllChannelsExcept()
        {
            Guid taskId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCompleted);
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.TaskCompleted);

            TaskCompletedEventArgs args = Helpers.WaitForEvent<TaskCompletedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCompleted += handler,
                () => this.Sender.NotifyTaskCompleted(taskId, timestamp, Guid.NewGuid(), false, TimeSpan.FromSeconds(2)));

            Assert.AreEqual(taskId, args.TaskId);
            Assert.AreEqual(timestamp, args.TimestampUtc);
            Assert.AreEqual(TimeSpan.FromSeconds(2), args.TotalCpuTime);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCompletedAfterUnsubscribeFromAllChannelsExcept()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.TaskCompleted);
            this.Receiver.UnsubscribeFromAllChannelsExcept();

            Helpers.WaitForEvent<TaskCompletedEventArgs>(
                this.Timeout,
                handler => this.Receiver.TaskCompleted += handler,
                () => this.Sender.NotifyTaskCompleted(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), false, TimeSpan.FromSeconds(1)));
        }

        #endregion TaskCompleted

        #region Master Commands

        [TestMethod]
        public void AddTaskSubmittedMasterCommand()
        {
            Guid taskId = Guid.NewGuid();

            this.Sender.NotifyTaskSubmitted(taskId, DateTime.UtcNow, false);

            Thread.Sleep(this.Timeout);

            Assert.IsTrue(this.MasterCommands
                .OfType<TaskSubmittedMasterCommand>()
                .Any(c => c.TaskId == taskId));
        }

        [TestMethod]
        public void DoNotAddTaskRequestedMasterCommandForPollingQueueTask()
        {
            Guid taskId = Guid.NewGuid();

            this.Sender.NotifyTaskSubmitted(taskId, DateTime.UtcNow, true);

            Thread.Sleep(this.Timeout);

            Assert.IsFalse(this.MasterCommands
                .OfType<TaskSubmittedMasterCommand>()
                .Any(c => c.TaskId == taskId));
        }

        [TestMethod]
        public void AddTaskCompletedMasterCommand()
        {
            Guid taskId = Guid.NewGuid();

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.Sender.NotifyTaskCompleted(taskId, timestamp, taskProcessorId, true, TimeSpan.FromMinutes(1));

            Thread.Sleep(this.Timeout);

            TaskCompletedMasterCommand command = this.MasterCommands.OfType<TaskCompletedMasterCommand>().First(c => c.TaskId == taskId);

            Assert.AreEqual(taskProcessorId, command.TaskProcessorId);

            this.AssertEquals(timestamp, command.TimestampUtc);

            Assert.AreEqual(TimeSpan.FromMinutes(1), command.TotalCpuTime);

            Assert.IsTrue(command.IsTaskProcessorStopping);
        }

        [TestMethod]
        public void AddTaskFailedMasterCommand()
        {
            Guid taskId = Guid.NewGuid();

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.Sender.NotifyTaskFailed(taskId, timestamp, taskProcessorId, false, "Hello Error");

            Thread.Sleep(this.Timeout);

            TaskFailedMasterCommand command = this.MasterCommands.OfType<TaskFailedMasterCommand>().First(c => c.TaskId == taskId);

            Assert.AreEqual(taskProcessorId, command.TaskProcessorId);
            Assert.IsFalse(command.IsTaskProcessorStopping);
            Assert.AreEqual("Hello Error", command.Error);

            this.AssertEquals(timestamp, command.TimestampUtc);
        }

        [TestMethod]
        public void AddTaskCancelCompletedMasterCommand()
        {
            Guid taskId = Guid.NewGuid();

            Guid taskProcessorId = Guid.NewGuid();

            DateTime timestamp = DateTime.UtcNow;

            this.Sender.NotifyTaskCancelCompleted(taskId, timestamp, taskProcessorId, true);

            Thread.Sleep(this.Timeout);

            TaskCancelCompletedMasterCommand command = this.MasterCommands.OfType<TaskCancelCompletedMasterCommand>().First(c => c.TaskId == taskId);

            Assert.AreEqual(taskProcessorId, command.TaskProcessorId);

            this.AssertEquals(timestamp, command.TimestampUtc);

            Assert.IsTrue(command.IsTaskProcessorStopping);
        }

        #endregion Master Commands

        protected virtual void AssertEquals(DateTime value1, DateTime value2)
        {
            Assert.AreEqual(value1, value2);
        }
    }
}