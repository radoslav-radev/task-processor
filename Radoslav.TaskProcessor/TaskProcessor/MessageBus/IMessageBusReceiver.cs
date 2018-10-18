using System;
using System.Collections.Generic;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Basic functionality of a message bus receiver.
    /// </summary>
    public interface IMessageBusReceiver
    {
        /// <summary>
        /// Gets or sets the subscribe and unsubscribe timeout.
        /// </summary>
        /// <value>The subscribe and unsubscribe timeout.</value>
        /// <exception cref="ArgumentOutOfRangeException">Value is less then <see cref="TimeSpan.Zero"/>.</exception>
        TimeSpan SubscribeTimeout { get; set; }

        /// <summary>
        /// Subscribes for channels of the specified types.
        /// </summary>
        /// <param name="channelTypes">The channel types to subscribe to.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="channelTypes"/> is null.</exception>
        void SubscribeForChannels(IEnumerable<MessageBusChannel> channelTypes);

        /// <summary>
        /// Un-subscribes from channels of the specified types.
        /// </summary>
        /// <param name="channelTypes">The channel types to un-subscribe from.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="channelTypes"/> is null.</exception>
        void UnsubscribeFromChannels(IEnumerable<MessageBusChannel> channelTypes);

        /// <summary>
        /// Un-subscribes from all channels.
        /// </summary>
        void UnsubscribeFromAllChannels();

        /// <summary>
        /// Un-subscribes from all channels except the specified ones.
        /// </summary>
        /// <param name="channelTypes">The channel types not to un-subscribe from.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="channelTypes"/> is null.</exception>
        void UnsubscribeFromAllChannelsExcept(IEnumerable<MessageBusChannel> channelTypes);
    }
}