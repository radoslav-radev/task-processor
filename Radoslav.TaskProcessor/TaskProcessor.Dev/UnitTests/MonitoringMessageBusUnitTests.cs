using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Redis;
using Radoslav.TaskProcessor.MessageBus;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class MonitoringMessageBusUnitTests : MessageBusTestsBase
    {
        [TestCleanup]
        public void TestCleanup()
        {
            foreach (ITaskProcessorMessageBus messageBus in (CompositeTaskProcessorMessageBus)this.MessageBus)
            {
                if (messageBus is IDisposable)
                {
                    ((IDisposable)messageBus).Dispose();
                }

                if (messageBus is RedisTaskProcessorMessageBus)
                {
                    ((RedisTaskProcessorMessageBus)messageBus).Provider.Dispose();
                }
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = SuppressMessages.DisposedOnTestCleanup)]
        protected override ITaskProcessorMessageBus CreateMessageBus()
        {
            CompositeTaskProcessorMessageBus result = new CompositeTaskProcessorMessageBus();

            ServiceStackRedisProvider provider = new ServiceStackRedisProvider();

            result.Add(new RedisTaskProcessorMessageBus(provider));
            result.Add(new RedisMonitoringMessageBus(provider));

            return result;
        }
    }
}