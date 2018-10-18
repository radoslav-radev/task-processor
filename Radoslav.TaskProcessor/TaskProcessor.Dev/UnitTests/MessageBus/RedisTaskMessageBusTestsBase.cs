using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.MessageBus.Redis;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class RedisTaskMessageBusTestsBase : TaskMessageBusTests
    {
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
        public override void RaiseTaskSubmitted()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskSubmittedAfterUnsubscribeFromAllChannelsExcept()
        {
            /* Do nothing. */
        }

        [TestMethod]
        public override void RaiseTaskSubmittedAfterUnsubscribeFromChannels()
        {
            /* Do nothing. */
        }

        public override void RaiseTaskCancelCompleted()
        {
            /* Do nothing. */
        }

        public override void RaiseTaskCancelCompletedAfterUnsubscribeFromChannels()
        {
            /* Do nothing. */
        }

        public override void RaiseTaskCancelCompletedAfterUnsubscribeFromAllChannelsExcept()
        {
            /* Do nothing. */
        }

        public override void RaiseTaskFailed()
        {
            /* Do nothing. */
        }

        public override void RaiseTaskFailedAfterUnsubscribeFromAllChannelsExcept()
        {
            /* Do nothing. */
        }

        public override void RaiseTaskFailedAfterUnsubscribeFromChannels()
        {
            /* Do nothing. */
        }

        protected abstract IRedisProvider GetProvider();

        protected override ITaskMessageBusSender CreateSender()
        {
            return new RedisTaskMessageBusSender(this.GetProvider(), this.MasterCommands);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        protected override ITaskMessageBusReceiver CreateReceiver()
        {
            return new RedisTaskMessageBusReceiver(this.GetProvider());
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        protected override ITaskProcessorMessageQueue CreateMasterCommandsQueue()
        {
            return new RedisTaskProcessorMessageQueue(this.GetProvider(), new FakeBinarySerializer());
        }
    }
}