using System;
using System.Collections.Generic;

namespace Radoslav.Redis
{
    /// <summary>
    /// Class for Redis extensions.
    /// </summary>
    public static class RedisExtensions
    {
        /// <summary>
        /// Subscribes to channels in order to receive messages on them.
        /// </summary>
        /// <param name="subscription">The <see cref="IRedisMessageSubscription"/> instance.</param>
        /// <param name="timeout">The timeout to wait for Redis to confirm that the subscribing to the specified channels is complete.</param>
        /// <param name="channels">The channels to subscribe to.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="subscription"/> is null, or <paramref name="channels"/> is null or empty array.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="timeout"/> is less or equal to <see cref="TimeSpan.Zero"/>.</exception>
        /// <exception cref="TimeoutException">Redis did not confirm that subscribing has completed within the specified timeout.</exception>
        public static void SubscribeToChannels(this IRedisMessageSubscription subscription, TimeSpan timeout, params string[] channels)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException("subscription");
            }

            if ((channels == null) || (channels.Length == 0))
            {
                throw new ArgumentNullException("channels");
            }

            subscription.SubscribeToChannels(timeout, (IEnumerable<string>)channels);
        }

        /// <summary>
        /// Unsubscribe from the specified channels.
        /// </summary>
        /// <param name="subscription">The <see cref="IRedisMessageSubscription"/> instance.</param>
        /// <param name="timeout">The timeout to wait for Redis to confirm that the unsubscribing to the specified channels is complete.</param>
        /// <param name="channels">The channels to unsubscribe from.</param>
        /// <returns>True if Redis confirmed unsubscribing to all specified channels within the specified timeout; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="subscription"/> is null, or <paramref name="channels"/> is null or empty array.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="timeout"/> is less or equal to <see cref="TimeSpan.Zero"/>.</exception>
        public static bool UnsubscribeFromChannels(this IRedisMessageSubscription subscription, TimeSpan timeout, params string[] channels)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException("subscription");
            }

            if ((channels == null) || (channels.Length == 0))
            {
                throw new ArgumentNullException("channels");
            }

            return subscription.UnsubscribeFromChannels(timeout, (IEnumerable<string>)channels);
        }
    }
}