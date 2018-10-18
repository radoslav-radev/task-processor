using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Radoslav.Redis;

namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    public abstract partial class RedisMessageBusReceiverBase : IDisposable
    {
        private readonly IDictionary<MessageBusChannel, string> channelsMapping = new Dictionary<MessageBusChannel, string>();
        private readonly IRedisProvider provider;
        private readonly IRedisMessageSubscription subscription;
        private readonly string debugName;
        private bool isDisposed;
        private TimeSpan subscribeTimeout = TimeSpan.FromSeconds(10);

        #region Constructor / Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisMessageBusReceiverBase"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> is null.</exception>
        protected RedisMessageBusReceiverBase(IRedisProvider provider)
        {
            this.debugName = this.GetType().Name;

            Trace.WriteLine("ENTER: Constructing {0} ...".FormatInvariant(this.debugName));

            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this.provider = provider;

            this.subscription = provider.CreateSubscription();

            this.subscription.MessageReceived += this.OnMessageReceived;

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.debugName));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RedisMessageBusReceiverBase"/> class.
        /// </summary>
        ~RedisMessageBusReceiverBase()
        {
            this.Dispose(false);
        }

        #endregion Constructor / Destructor

        #region Properties

        /// <summary>
        /// Gets the Redis provider used to receive messages.
        /// </summary>
        /// <value>The Redis provider used to receive messages.</value>
        public IRedisProvider Provider
        {
            get { return this.provider; }
        }

        #endregion Properties

        /// <summary>
        /// Serializes task processor performance info to <see cref="String"/>.
        /// </summary>
        /// <param name="performanceInfo">The performance info to serialize.</param>
        /// <returns>A string representing the task processor performance info.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="performanceInfo"/> is null.</exception>
        public static string SerializeTaskProcessorPerformanceInfo(TaskProcessorPerformanceReport performanceInfo)
        {
            if (performanceInfo == null)
            {
                throw new ArgumentNullException("performanceInfo");
            }

            Trace.WriteLine("ENTER: Serializing {0} ...".FormatInvariant(performanceInfo.GetType().Name));

            StringBuilder result = new StringBuilder();

            result.AppendFormat(
                "{0};{1};{2}",
                RedisConverter.ToString(performanceInfo.TaskProcessorId),
                RedisConverter.ToString(performanceInfo.CpuPercent),
                RedisConverter.ToString(performanceInfo.RamPercent));

            foreach (TaskPerformanceReport taskPerformance in performanceInfo.TasksPerformance)
            {
                result.AppendFormat(
                    "#{0};{1};{2}",
                    RedisConverter.ToString(taskPerformance.TaskId),
                    RedisConverter.ToString(taskPerformance.CpuPercent),
                    RedisConverter.ToString(taskPerformance.RamPercent));
            }

            Trace.WriteLine("EXIT: {0} serialized to {1}.".FormatInvariant(performanceInfo.GetType().Name, result));

            return result.ToString();
        }

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members

        /// <summary>
        /// De-serialize a <see cref="TaskProcessorPerformanceReport"/> instance from <see cref="String"/>.
        /// </summary>
        /// <param name="value">The string to de-serialize.</param>
        /// <returns>A <see cref="TaskProcessorPerformanceReport"/> instance de-serialized from the specified string.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="value"/> is null or empty string.</exception>
        protected static TaskProcessorPerformanceReport DeserializeTaskProcessorPerformanceInfo(string value)
        {
            Trace.WriteLine("ENTER: De-serializing '{0}' to {1} ...".FormatInvariant(value, typeof(TaskProcessorPerformanceReport).Name));

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            string[] values1 = value.Split('#');

            string[] values2 = values1[0].Split(';');

            TaskProcessorPerformanceReport result = new TaskProcessorPerformanceReport(RedisConverter.ParseGuid(values2[0]))
            {
                CpuPercent = RedisConverter.ParseInteger(values2[1]),
                RamPercent = RedisConverter.ParseInteger(values2[2]),
            };

            for (int i = 1; i < values1.Length; i++)
            {
                values2 = values1[i].Split(';');

                result.TasksPerformance.Add(new TaskPerformanceReport(RedisConverter.ParseGuid(values2[0]))
                {
                    CpuPercent = RedisConverter.ParseFloat(values2[1]),
                    RamPercent = RedisConverter.ParseFloat(values2[2]),
                });
            }

            Trace.WriteLine("EXIT: '{0}' de-serialized to {1}.".FormatInvariant(value, typeof(TaskProcessorPerformanceReport).Name));

            return result;
        }

        #region Receive Message

        /// <summary>
        /// Handles a message received on a channel by the message bus.
        /// </summary>
        /// <param name="sender">The subscription raising the event.</param>
        /// <param name="e">The event arguments.</param>
        protected abstract void OnMessageReceived(object sender, RedisMessageEventArgs e);

        /// <summary>
        /// Raises a <see cref="ITaskProcessorMessageBus"/> event from a message received on channel.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments for the event.</typeparam>
        /// <typeparam name="T1">The type of the first argument in the message.</typeparam>
        /// <param name="e">The event arguments for the received message.</param>
        /// <param name="handler">The <see cref="ITaskProcessorMessageBus"/> event to be raised.</param>
        /// <param name="converter">Callback method to convert the arguments in the message to event arguments.</param>
        protected void RaiseEventFromMessage<TEventArgs, T1>(RedisMessageEventArgs e, EventHandler<TEventArgs> handler, Func<T1, TEventArgs> converter)
             where TEventArgs : EventArgs
        {
            this.RaiseEventFromMessage<TEventArgs, T1, object, object, object>(e, handler, (v1, v2, v3, v4) => converter(v1));
        }

        /// <summary>
        /// Raises a <see cref="ITaskProcessorMessageBus"/> event from a message received on channel.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments for the event.</typeparam>
        /// <typeparam name="T1">The type of the first argument in the message.</typeparam>
        /// <typeparam name="T2">The type of the second argument in the message.</typeparam>
        /// <param name="e">The event arguments for the received message.</param>
        /// <param name="handler">The <see cref="ITaskProcessorMessageBus"/> event to be raised.</param>
        /// <param name="converter">Callback method to convert the arguments in the message to event arguments.</param>
        protected void RaiseEventFromMessage<TEventArgs, T1, T2>(RedisMessageEventArgs e, EventHandler<TEventArgs> handler, Func<T1, T2, TEventArgs> converter)
             where TEventArgs : EventArgs
        {
            this.RaiseEventFromMessage<TEventArgs, T1, T2, object, object>(e, handler, (v1, v2, v3, v4) => converter(v1, v2));
        }

        /// <summary>
        /// Raises a <see cref="ITaskProcessorMessageBus"/> event from a message received on channel.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments for the event.</typeparam>
        /// <typeparam name="T1">The type of the first argument in the message.</typeparam>
        /// <typeparam name="T2">The type of the second argument in the message.</typeparam>
        /// <typeparam name="T3">The type of the third argument in the message.</typeparam>
        /// <param name="e">The event arguments for the received message.</param>
        /// <param name="handler">The <see cref="ITaskProcessorMessageBus"/> event to be raised.</param>
        /// <param name="converter">Callback method to convert the arguments in the message to event arguments.</param>
        protected void RaiseEventFromMessage<TEventArgs, T1, T2, T3>(RedisMessageEventArgs e, EventHandler<TEventArgs> handler, Func<T1, T2, T3, TEventArgs> converter)
             where TEventArgs : EventArgs
        {
            this.RaiseEventFromMessage<TEventArgs, T1, T2, T3, object>(e, handler, (v1, v2, v3, v4) => converter(v1, v2, v3));
        }

        /// <summary>
        /// Raises a <see cref="ITaskProcessorMessageBus"/> event from a message received on channel.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments for the event.</typeparam>
        /// <typeparam name="T1">The type of the first argument in the message.</typeparam>
        /// <typeparam name="T2">The type of the second argument in the message.</typeparam>
        /// <typeparam name="T3">The type of the third argument in the message.</typeparam>
        /// <typeparam name="T4">The type of the forth argument in the message.</typeparam>
        /// <param name="e">The event arguments for the received message.</param>
        /// <param name="handler">The <see cref="ITaskProcessorMessageBus"/> event to be raised.</param>
        /// <param name="converter">Callback method to convert the arguments in the message to event arguments.</param>
        protected void RaiseEventFromMessage<TEventArgs, T1, T2, T3, T4>(RedisMessageEventArgs e, EventHandler<TEventArgs> handler, Func<T1, T2, T3, T4, TEventArgs> converter)
             where TEventArgs : EventArgs
        {
            if (handler == null)
            {
                return;
            }

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            string[] values = RedisConverter.ParseCollection<string>(e.Message).ToArray();

            T1 value1;
            T2 value2;
            T3 value3;
            T4 value4;

            if (values.Length > 0)
            {
                value1 = RedisConverter.ParseValue<T1>(values[0]);
            }
            else
            {
                value1 = default(T1);
            }

            if (values.Length > 1)
            {
                value2 = RedisConverter.ParseValue<T2>(values[1]);
            }
            else
            {
                value2 = default(T2);
            }

            if (values.Length > 2)
            {
                value3 = RedisConverter.ParseValue<T3>(values[2]);
            }
            else
            {
                value3 = default(T3);
            }

            if (values.Length > 3)
            {
                value4 = RedisConverter.ParseValue<T4>(values[3]);
            }
            else
            {
                value4 = default(T4);
            }

            TEventArgs args = converter(value1, value2, value3, value4);

            ThreadPool.QueueUserWorkItem(state => handler(this, args));
        }

        #endregion Receive Message

        /// <summary>
        /// Adds a mapping between a channel type and a channel name.
        /// </summary>
        /// <param name="channelType">The type of the channel.</param>
        /// <param name="channelName">The name of the channel.</param>
        protected void AddChannelMapping(MessageBusChannel channelType, string channelName)
        {
            if (channelType == MessageBusChannel.None)
            {
                throw new ArgumentOutOfRangeException("channelType", channelType, "Value must not be None.");
            }

            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException("channelName");
            }

            this.channelsMapping.Add(channelType, channelName);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="isDisposing">True if called from the <see cref="IDisposable.Dispose"/> method; false if called from the destructor.</param>
        protected virtual void Dispose(bool isDisposing)
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

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.debugName);
            }
        }
    }
}