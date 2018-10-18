using System;
using System.Collections.Generic;
using System.Linq;
using Radoslav.TaskProcessor.MessageBus;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal abstract class FakeMessageBusBase : MockObject
    {
        protected readonly FakeMessageQueue MasterCommands;

        protected readonly HashSet<MessageBusChannel> SubscribedChannels = new HashSet<MessageBusChannel>();

        protected FakeMessageBusBase()
        {
            this.MasterCommands = new FakeMessageQueue();
        }

        protected FakeMessageBusBase(FakeMessageQueue masterCommands)
        {
            this.MasterCommands = masterCommands;
        }

        #region ITaskProcessorMessageBusBase Members

        public virtual TimeSpan SubscribeTimeout { get; set; }

        public virtual void SubscribeForChannels(IEnumerable<MessageBusChannel> channelTypes)
        {
            if (channelTypes == null)
            {
                throw new ArgumentNullException("channelTypes");
            }

            this.RecordMethodCall(channelTypes);

            foreach (MessageBusChannel channel in channelTypes)
            {
                this.SubscribedChannels.Add(channel);
            }
        }

        public virtual void UnsubscribeFromChannels(IEnumerable<MessageBusChannel> channelTypes)
        {
            if (channelTypes == null)
            {
                throw new ArgumentNullException("channelTypes");
            }

            this.RecordMethodCall(channelTypes);

            foreach (MessageBusChannel channel in channelTypes)
            {
                this.SubscribedChannels.Remove(channel);
            }
        }

        public virtual void UnsubscribeFromAllChannels()
        {
            this.RecordMethodCall();

            this.SubscribedChannels.Clear();

            this.MasterCommands.ReceiveMessages = false;
        }

        public virtual void UnsubscribeFromAllChannelsExcept(IEnumerable<MessageBusChannel> channelTypes)
        {
            this.RecordMethodCall(channelTypes);

            this.SubscribedChannels.RemoveWhere(c => !channelTypes.Contains(c));
        }

        #endregion ITaskProcessorMessageBusBase Members
    }
}