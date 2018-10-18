using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Radoslav.Redis;
using Radoslav.Serialization;

namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This is a message queue implementation.")]
    public sealed partial class RedisTaskProcessorMessageQueue : IDisposable
    {
        private const string MasterCommandsChannel = "Radoslav$TaskProcessor$MasterCommand";
        private const string MessageQueueListKey = "MasterCommandsList";
        private const string MessageQueueContentKey = "MasterCommandsContent";

        private readonly IRedisProvider provider;
        private readonly IEntityBinarySerializer serializer;
        private readonly string debugName = typeof(RedisTaskProcessorMessageQueue).Name;
        private readonly IRedisMessageSubscription subscription;

        private TimeSpan subscribeTimeout = TimeSpan.FromSeconds(5);
        private bool isDisposed;

        #region Constructor & Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskProcessorMessageQueue"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <param name="serializer">The serializer to use for master commands serialization.</param>
        public RedisTaskProcessorMessageQueue(IRedisProvider provider, IEntityBinarySerializer serializer)
        {
            Trace.WriteLine("ENTER: Constructing {0} ...".FormatInvariant(this.debugName));

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            this.provider = provider;
            this.serializer = serializer;

            this.subscription = provider.CreateSubscription();

            this.subscription.MessageReceived += this.OnMessageReceived;

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.debugName));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RedisTaskProcessorMessageQueue"/> class.
        /// </summary>
        ~RedisTaskProcessorMessageQueue()
        {
            this.Dispose(false);
        }

        #endregion Constructor & Destructor

        /// <summary>
        /// Gets or sets the subscribe and unsubscribe timeout.
        /// </summary>
        /// <value>The subscribe and unsubscribe timeout.</value>
        /// <exception cref="ArgumentOutOfRangeException">Value is less then <see cref="TimeSpan.Zero"/>.</exception>
        public TimeSpan SubscribeTimeout
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

        #region IEnumerable<TMessage> Members

        /// <inheritdoc />
        public IEnumerator<IUniqueMessage> GetEnumerator()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            if (this.serializer.CanDetermineEntityTypeFromContent)
            {
                return this.provider.GetListAsBinary(RedisTaskProcessorMessageQueue.MessageQueueListKey)
                    .Select(content => this.serializer.Deserialize(content))
                    .Cast<IUniqueMessage>()
                    .GetEnumerator();
            }
            else
            {
                return this.provider.GetListAsText(RedisTaskProcessorMessageQueue.MessageQueueListKey)
                    .Select(messageUniqueKey => this.GetByUniqueKey(messageUniqueKey, false))
                    .GetEnumerator();
            }
        }

        #endregion IEnumerable<TMessage> Members

        #region IEnumerable Members

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable Members

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members

        private void RaiseMessageReceived()
        {
            EventHandler messageReceived = this.MessageReceived;

            if (messageReceived != null)
            {
                messageReceived(this, EventArgs.Empty);
            }
        }

        private IUniqueMessage GetByUniqueKey(string messageUniqueKey, bool deleteAfterRead)
        {
            byte[] content = null;
            string messageTypeAsString = null;

            using (IRedisTransaction transaction = this.provider.CreateTransaction())
            {
                transaction.GetHashBinaryValue(RedisTaskProcessorMessageQueue.MessageQueueContentKey, messageUniqueKey, value => content = value);
                transaction.GetHashTextValue(RedisTaskProcessorMessageQueue.MessageQueueContentKey, messageUniqueKey + "$Type", value => messageTypeAsString = value);

                if (deleteAfterRead)
                {
                    transaction.RemoveFromHash(RedisTaskProcessorMessageQueue.MessageQueueContentKey, messageUniqueKey, messageUniqueKey + "$Type");
                }

                transaction.Commit();
            }

            if (content == null)
            {
                // TODO: Trace warning ...
                return null;
            }

            if (string.IsNullOrEmpty(messageTypeAsString))
            {
                // TODO: Trace warning ...
                return null;
            }

            Type messageType = Type.GetType(messageTypeAsString, false);

            if (messageType == null)
            {
                // TODO: Trace warning ...
                return null;
            }

            return (IUniqueMessage)this.serializer.Deserialize(content, messageType);
        }

        private void Dispose(bool isDisposing)
        {
            Trace.WriteLine("ENTER: Disposing {0} ...".FormatInvariant(this.debugName));

            if (!isDisposing)
            {
                Trace.WriteLine("ENTER: Destructing {0} ...".FormatInvariant(this.debugName));
            }

            if (this.isDisposed)
            {
                Trace.WriteLine("EXIT: {0} is already disposed.".FormatInvariant(this.debugName));

                return;
            }

            if (this.subscription != null)
            {
                this.subscription.MessageReceived -= this.OnMessageReceived;

                this.subscription.Dispose();
            }

            this.isDisposed = true;

            Trace.WriteLine("EXIT: {0} disposed.".FormatInvariant(this.debugName));

            if (!isDisposing)
            {
                Trace.WriteLine("EXIT: {0} destructed.".FormatInvariant(this.debugName));
            }
        }

        private void OnMessageReceived(object sender, RedisMessageEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(_ => this.RaiseMessageReceived());
        }
    }
}