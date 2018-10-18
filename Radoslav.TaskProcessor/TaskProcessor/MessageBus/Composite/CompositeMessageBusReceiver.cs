using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// A composite implementation of <see cref="IMessageBusReceiver"/>.
    /// </summary>
    /// <typeparam name="TMessageBusReceiver">The type of the elements in the composite.</typeparam>
    public abstract class CompositeMessageBusReceiver<TMessageBusReceiver> : Collection<TMessageBusReceiver>, IMessageBusReceiver
        where TMessageBusReceiver : IMessageBusReceiver
    {
        #region IMessageBusReceiver Members

        /// <inheritdoc />
        public TimeSpan SubscribeTimeout
        {
            get
            {
                var result = this
                    .GroupBy(mb => mb.SubscribeTimeout)
                    .Take(2)
                    .ToArray();

                switch (result.Length)
                {
                    case 0:
                        throw new InvalidOperationException("Composite message bus is empty.");

                    case 1:
                        return result[0].Key;

                    default:
                        throw new InvalidOperationException("Composite message bus has different subscribe timeout values.");
                }
            }

            set
            {
                foreach (IMessageBusReceiver mb in this)
                {
                    mb.SubscribeTimeout = value;
                }
            }
        }

        /// <inheritdoc />
        public void SubscribeForChannels(IEnumerable<MessageBusChannel> channelTypes)
        {
            foreach (IMessageBusReceiver mb in this)
            {
                mb.SubscribeForChannels(channelTypes);
            }
        }

        /// <inheritdoc />
        public void UnsubscribeFromChannels(IEnumerable<MessageBusChannel> channelTypes)
        {
            foreach (IMessageBusReceiver mb in this)
            {
                mb.UnsubscribeFromChannels(channelTypes);
            }
        }

        /// <inheritdoc />
        public void UnsubscribeFromAllChannels()
        {
            foreach (IMessageBusReceiver mb in this)
            {
                mb.UnsubscribeFromAllChannels();
            }
        }

        /// <inheritdoc />
        public void UnsubscribeFromAllChannelsExcept(IEnumerable<MessageBusChannel> channelTypes)
        {
            foreach (IMessageBusReceiver mb in this)
            {
                mb.UnsubscribeFromAllChannelsExcept(channelTypes);
            }
        }

        #endregion IMessageBusReceiver Members

        /// <inheritdoc />
        protected override void InsertItem(int index, TMessageBusReceiver item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            base.InsertItem(index, item);
        }

        /// <inheritdoc />
        protected override void SetItem(int index, TMessageBusReceiver item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            base.SetItem(index, item);
        }
    }
}