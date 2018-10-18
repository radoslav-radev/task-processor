using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.MessageBus.Redis;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class RedisTaskProcessorsMessageBusTestsBase : TaskProcessorsMessageBusTests
    {
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

        protected abstract IRedisProvider GetProvider();

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        protected override ITaskProcessorMessageQueue CreateMasterCommandsQueue()
        {
            return new RedisTaskProcessorMessageQueue(this.GetProvider(), new FakeBinarySerializer());
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        protected override ITaskProcessorMessageBusReceiver CreateReceiver()
        {
            return new RedisTaskProcessorMessageBusReceiver(this.GetProvider());
        }

        protected override ITaskProcessorMessageBusSender CreateSender()
        {
            return new RedisTaskProcessorMessageBusSender(this.GetProvider(), this.MasterCommands);
        }
    }
}