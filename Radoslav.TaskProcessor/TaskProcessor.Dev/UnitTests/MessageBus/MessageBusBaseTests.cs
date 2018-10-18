using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.MessageBus;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class MessageBusBaseTests<TSender, TReceiver>
        where TReceiver : IMessageBusReceiver
    {
        #region Properties & Initialize

        public TestContext TestContext { get; set; }

        protected virtual TimeSpan Timeout
        {
            get
            {
                if (Debugger.IsAttached)
                {
                    return TimeSpan.FromSeconds(2);
                }
                else
                {
                    return TimeSpan.FromSeconds(2);
                }
            }
        }

        protected TSender Sender
        {
            get
            {
                return (TSender)this.TestContext.Properties["Sender"];
            }
        }

        protected TReceiver Receiver
        {
            get
            {
                return (TReceiver)this.TestContext.Properties["Receiver"];
            }
        }

        protected ITaskProcessorMessageQueue MasterCommands
        {
            get
            {
                return (ITaskProcessorMessageQueue)this.TestContext.Properties["MasterCommands"];
            }
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            this.TestContext.Properties.Add("MasterCommands", this.CreateMasterCommandsQueue());
            this.TestContext.Properties.Add("Receiver", this.CreateReceiver());
            this.TestContext.Properties.Add("Sender", this.CreateSender());
        }

        #endregion Properties & Initialize

        #region SubscribeTimeout

        [TestMethod]
        public void SubscribeTimeout()
        {
            TimeSpan value = DateTime.UtcNow.TimeOfDay;

            this.Receiver.SubscribeTimeout = value;

            Assert.AreEqual(value, this.Receiver.SubscribeTimeout);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SubscribeTimeoutNegative()
        {
            this.Receiver.SubscribeTimeout = TimeSpan.FromMinutes(-1);
        }

        #endregion SubscribeTimeout

        #region Subscribe / Unsubscribe

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SubscribeForChannelsNull()
        {
            this.Receiver.SubscribeForChannels(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnsubscribeFromChannelsNull()
        {
            this.Receiver.UnsubscribeFromChannels(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnsubscribeFromAllChannelsExceptNull()
        {
            this.Receiver.UnsubscribeFromAllChannelsExcept(null);
        }

        [TestMethod]
        public void SubscribeForUnknownChannel()
        {
            this.Receiver.SubscribeForChannels(MessageBusChannel.None);
        }

        [TestMethod]
        public void UnsubscribeFromUnknownChannels()
        {
            this.Receiver.UnsubscribeFromChannels(MessageBusChannel.None);
        }

        [TestMethod]
        public void UnsubscribeFromAllChannelsExceptUnknown()
        {
            this.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.None);
        }

        #endregion Subscribe / Unsubscribe

        protected abstract TSender CreateSender();

        protected abstract TReceiver CreateReceiver();

        protected abstract ITaskProcessorMessageQueue CreateMasterCommandsQueue();
    }
}