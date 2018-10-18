using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.MessageBus;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class CompositeMessageBusUnitTests
    {
        [TestMethod]
        public void GetTaskSender()
        {
            CompositeTaskMessageBus composite = new CompositeTaskMessageBus();

            composite.Senders.Add(new FakeTaskMessageBus());
            composite.Senders.Add(new FakeTaskMessageBus());

            composite.Senders.OfType<FakeTaskMessageBus>().First().PredefineResult(false, mb => mb.IsSupported(typeof(FakeTask)));
            composite.Senders.OfType<FakeTaskMessageBus>().Last().PredefineResult(true, mb => mb.IsSupported(typeof(FakeTask)));

            ITaskMessageBusSender result = composite.GetSender<FakeTask>();

            result.NotifyTaskProgress(Guid.Empty, 0);

            composite.Senders.OfType<FakeTaskMessageBus>().First().AssertNoMethodCall(mb => mb.NotifyTaskProgress(Guid.Empty, 0));
            composite.Senders.OfType<FakeTaskMessageBus>().Last().AssertMethodCallOnce(mb => mb.NotifyTaskProgress(Guid.Empty, 0));
        }
    }
}