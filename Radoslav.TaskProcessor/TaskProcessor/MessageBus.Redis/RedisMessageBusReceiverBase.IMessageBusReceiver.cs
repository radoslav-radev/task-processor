using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    /// <summary>
    /// A basic implementation of <see cref="IMessageBusReceiver"/> that uses Redis for communication.
    /// </summary>
    public abstract partial class RedisMessageBusReceiverBase : IMessageBusReceiver
    {
        #region IMessageBusReceiver Members

        /// <inheritdoc />
        public virtual TimeSpan SubscribeTimeout
        {
            get
            {
                return this.subscribeTimeout;
            }

            set
            {
                Trace.WriteLine("ENTER: Setting {0} SubscribeTimeout to {1} ...".FormatInvariant(this.debugName, value));

                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Value must be positive.");
                }

                this.subscribeTimeout = value;

                Trace.WriteLine("EXIT: {0} SubscribeTimeout set to {1}.".FormatInvariant(this.debugName, value));
            }
        }

        /// <inheritdoc />
        public virtual void SubscribeForChannels(IEnumerable<MessageBusChannel> channelTypes)
        {
            Trace.WriteLine("ENTER: Subscribing for channels {0} ...".FormatInvariant(channelTypes.ToString(", ")));

            this.ThrowIfDisposed();

            var supportedChannels = channelTypes
                .Where(c => this.channelsMapping.ContainsKey(c))
                .Select(c => this.channelsMapping[c]);

            this.subscription.SubscribeToChannels(this.subscribeTimeout, supportedChannels);

            Trace.WriteLine("EXIT: Subscribed for channels {0}.".FormatInvariant(channelTypes.ToString(", ")));
        }

        /// <inheritdoc />
        public virtual void UnsubscribeFromChannels(IEnumerable<MessageBusChannel> channelTypes)
        {
            Trace.WriteLine("ENTER: Unsubscribing from channels {0} ...".FormatInvariant(channelTypes.ToString(", ")));

            this.ThrowIfDisposed();

            var supportedChannels = channelTypes
                .Where(c => this.channelsMapping.ContainsKey(c))
                .Select(c => this.channelsMapping[c]);

            this.subscription.UnsubscribeFromChannels(this.subscribeTimeout, supportedChannels);

            Trace.WriteLine("EXIT: Unsubscribed from channels {0}.".FormatInvariant(channelTypes.ToString(", ")));
        }

        /// <inheritdoc />
        public virtual void UnsubscribeFromAllChannels()
        {
            Trace.WriteLine("ENTER: Unsubscribing from all channels ...");

            this.ThrowIfDisposed();

            this.subscription.UnsubscribeFromChannels(this.subscribeTimeout, this.subscription.ActiveChannels);

            Trace.WriteLine("EXIT: Unsubscribed from all channels.");
        }

        /// <inheritdoc />
        public virtual void UnsubscribeFromAllChannelsExcept(IEnumerable<MessageBusChannel> channelTypes)
        {
            Trace.WriteLine("ENTER: Unsubscribing from all channels except {0} ...".FormatInvariant(channelTypes.ToString(", ")));

            this.ThrowIfDisposed();

            var supportedChannels = channelTypes
                .Where(c => this.channelsMapping.ContainsKey(c))
                .Select(c => this.channelsMapping[c]);

            this.subscription.UnsubscribeFromChannels(this.subscribeTimeout, this.subscription.ActiveChannels.Except(supportedChannels));

            Trace.WriteLine("EXIT: Unsubscribed from all channels except {0}.".FormatInvariant(channelTypes.ToString(", ")));
        }

        #endregion IMessageBusReceiver Members
    }
}