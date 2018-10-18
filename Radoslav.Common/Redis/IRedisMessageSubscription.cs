using System;
using System.Collections.Generic;

namespace Radoslav.Redis
{
    /// <summary>
    /// Basic functionality of a Redis message subscription.
    /// </summary>
    public interface IRedisMessageSubscription : IDisposable
    {
        /// <summary>
        /// An event raised when a message is received on channel.
        /// </summary>
        event EventHandler<RedisMessageEventArgs> MessageReceived;

        /// <summary>
        /// Gets the channels on which the subscription is currently listening for messages.
        /// </summary>
        /// <value>The channels on which the subscriptions is currently listening for messages.</value>
        IEnumerable<string> ActiveChannels { get; }

        /// <summary>
        /// Subscribes to channels in order to receive messages on them.
        /// </summary>
        /// <param name="timeout">The timeout to wait for Redis to confirm that the subscribing to the specified channels is complete.</param>
        /// <param name="channels">The channels to subscribe to.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="channels"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="timeout"/> is less or equal to <see cref="TimeSpan.Zero"/>.</exception>
        /// <exception cref="TimeoutException">Redis did not confirm that subscribing has completed within the specified timeout.</exception>
        void SubscribeToChannels(TimeSpan timeout, IEnumerable<string> channels);

        /// <summary>
        /// Unsubscribe from the specified channels.
        /// </summary>
        /// <param name="timeout">The timeout to wait for Redis to confirm that the unsubscribing to the specified channels is complete.</param>
        /// <param name="channels">The channels to unsubscribe from.</param>
        /// <returns>True if Redis confirmed unsubscribing to all specified channels within the specified timeout; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="channels"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="timeout"/> is less or equal to <see cref="TimeSpan.Zero"/>.</exception>
        bool UnsubscribeFromChannels(TimeSpan timeout, IEnumerable<string> channels);
    }
}