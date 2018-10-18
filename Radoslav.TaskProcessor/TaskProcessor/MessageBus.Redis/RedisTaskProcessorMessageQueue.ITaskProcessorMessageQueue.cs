using System;
using System.Diagnostics;
using System.Linq;
using Radoslav.Redis;

namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    /// <summary>
    /// An implementation of the <see cref="ITaskProcessorMessageQueue"/> interface that uses Redis as storage and channel for communication.
    /// </summary>
    public sealed partial class RedisTaskProcessorMessageQueue : ITaskProcessorMessageQueue
    {
        #region ITaskProcessorMessageQueue Members

        /// <inheritdoc />
        public event EventHandler MessageReceived;

        /// <inheritdoc />
        public bool ReceiveMessages
        {
            get
            {
                return this.subscription.ActiveChannels.Any();
            }

            set
            {
                if (value)
                {
                    if (!this.ReceiveMessages)
                    {
                        if (this.isDisposed)
                        {
                            throw new ObjectDisposedException(this.GetType().Name);
                        }

                        this.subscription.SubscribeToChannels(this.subscribeTimeout, RedisTaskProcessorMessageQueue.MasterCommandsChannel);
                    }
                }
                else
                {
                    if (this.ReceiveMessages)
                    {
                        if (!this.isDisposed)
                        {
                            this.subscription.UnsubscribeFromChannels(this.subscribeTimeout, RedisTaskProcessorMessageQueue.MasterCommandsChannel);
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Push(IUniqueMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (string.IsNullOrEmpty(message.MessageUniqueId))
            {
                throw new ArgumentException("MessageUniqueId is null or empty.".FormatInvariant(message), "message");
            }

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            Trace.WriteLine("ENTER: Adding message {0}:{1} to list '{2}' ...".FormatInvariant(message.GetType().Name, message.MessageUniqueId, RedisTaskProcessorMessageQueue.MessageQueueListKey));

            byte[] content = this.serializer.Serialize(message);

            if (this.serializer.CanDetermineEntityTypeFromContent)
            {
                this.provider.AddToList(RedisTaskProcessorMessageQueue.MessageQueueListKey, content);
            }
            else
            {
                string messageUniqueId = string.Join("$", message.GetType().Name, message.MessageUniqueId);

                using (IRedisTransaction transaction = this.provider.CreateTransaction())
                {
                    transaction.AddToList(RedisTaskProcessorMessageQueue.MessageQueueListKey, messageUniqueId);

                    transaction.SetHashValue(RedisTaskProcessorMessageQueue.MessageQueueContentKey, messageUniqueId, content);
                    transaction.SetHashValue(RedisTaskProcessorMessageQueue.MessageQueueContentKey, messageUniqueId + "$Type", RedisConverter.ToString(message.GetType(), false));

                    transaction.Commit();
                }
            }

            this.provider.PublishMessage(RedisTaskProcessorMessageQueue.MasterCommandsChannel, string.Empty);

            Trace.WriteLine("EXIT: Message {0}:{1} added to list '{2}'.".FormatInvariant(message.GetType().Name, message.MessageUniqueId, RedisTaskProcessorMessageQueue.MessageQueueListKey));
        }

        /// <inheritdoc />
        public IUniqueMessage PopFirst()
        {
            Trace.WriteLine("ENTER: Popping first message from queue '{0}' ...".FormatInvariant(RedisTaskProcessorMessageQueue.MessageQueueListKey));

            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            IUniqueMessage result;

            if (this.serializer.CanDetermineEntityTypeFromContent)
            {
                byte[] content = this.provider.PopFirstListElementAsBinary(RedisTaskProcessorMessageQueue.MessageQueueListKey);

                result = (IUniqueMessage)this.serializer.Deserialize(content);
            }
            else
            {
                string messageUniqueKey = this.provider.PopFirstListElementAsText(RedisTaskProcessorMessageQueue.MessageQueueListKey);

                if (string.IsNullOrEmpty(messageUniqueKey))
                {
                    Trace.WriteLine("ENTER: No messages found in queue '{0}'.".FormatInvariant(RedisTaskProcessorMessageQueue.MessageQueueListKey));

                    return null;
                }

                result = this.GetByUniqueKey(messageUniqueKey, true);
            }

            Trace.WriteLine("EXIT: First message '{0}' from queue '{1}'.".FormatInvariant(result, RedisTaskProcessorMessageQueue.MessageQueueListKey));

            return result;
        }

        #endregion ITaskProcessorMessageQueue Members
    }
}