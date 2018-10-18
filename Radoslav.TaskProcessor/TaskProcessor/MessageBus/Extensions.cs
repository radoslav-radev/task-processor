using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Class for message bus extension methods.
    /// </summary>
    public static class MessageBusExtensions
    {
        /// <summary>
        /// Subscribes for channels of the specified types.
        /// </summary>
        /// <param name="messageBus">The message bus instance to extend.</param>
        /// <param name="channelTypes">The channel types to subscribe to.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="messageBus"/> or <paramref name="channelTypes"/> is null.</exception>
        public static void SubscribeForChannels(this IMessageBusReceiver messageBus, params MessageBusChannel[] channelTypes)
        {
            if (messageBus == null)
            {
                throw new ArgumentNullException("messageBus");
            }

            if (channelTypes == null)
            {
                throw new ArgumentNullException("channelTypes");
            }

            messageBus.SubscribeForChannels((IEnumerable<MessageBusChannel>)channelTypes);
        }

        /// <summary>
        /// Un-subscribes from channels of the specified types.
        /// </summary>
        /// <param name="messageBus">The message bus instance to extend.</param>
        /// <param name="channelTypes">The channel types to un-subscribe from.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="messageBus"/> or <paramref name="channelTypes"/> is null.</exception>
        public static void UnsubscribeFromChannels(this IMessageBusReceiver messageBus, params MessageBusChannel[] channelTypes)
        {
            if (messageBus == null)
            {
                throw new ArgumentNullException("messageBus");
            }

            if (channelTypes == null)
            {
                throw new ArgumentNullException("channelTypes");
            }

            messageBus.UnsubscribeFromChannels((IEnumerable<MessageBusChannel>)channelTypes);
        }

        /// <summary>
        /// Un-subscribes from all channels except the specified ones.
        /// </summary>
        /// <param name="messageBus">The message bus instance to extend.</param>
        /// <param name="channelTypes">The channel types not to un-subscribe from.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="messageBus"/> or <paramref name="channelTypes"/> is null.</exception>
        public static void UnsubscribeFromAllChannelsExcept(this IMessageBusReceiver messageBus, params MessageBusChannel[] channelTypes)
        {
            if (messageBus == null)
            {
                throw new ArgumentNullException("messageBus");
            }

            if (channelTypes == null)
            {
                throw new ArgumentNullException("channelTypes");
            }

            messageBus.UnsubscribeFromAllChannelsExcept((IEnumerable<MessageBusChannel>)channelTypes);
        }

        /// <summary>
        /// Gets a task message bus sender for a specific task type.
        /// </summary>
        /// <typeparam name="TTask">The type of the task.</typeparam>
        /// <param name="messageBus">The task message bus to extend.</param>
        /// <returns>A task message bus sender for a specified task type, or null if the task type is not supported.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="messageBus"/> is null.</exception>
        public static ITaskMessageBusSender GetSender<TTask>(this ITaskMessageBus messageBus)
            where TTask : ITask
        {
            if (messageBus == null)
            {
                throw new ArgumentNullException(nameof(messageBus));
            }

            return messageBus.GetSender(typeof(TTask));
        }
    }
}