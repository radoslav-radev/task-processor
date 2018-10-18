using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Radoslav.Collections;
using ServiceStack.Redis;

namespace Radoslav.Redis.ServiceStack
{
    internal sealed partial class ServiceStackSubscription : IRedisMessageSubscription, IDisposable
    {
        private static readonly string UnsubscribeChannel = string.Concat("Radoslav$", Guid.NewGuid(), "$Unsubscribe");

        private readonly IRedisClient client;
        private readonly IRedisSubscription subscription;
        private readonly ConcurrentHashSet<string> channels = new ConcurrentHashSet<string>();
        private readonly ConcurrentHashSet<SubscribeOperationState> subscribeOperations = new ConcurrentHashSet<SubscribeOperationState>();
        private readonly string debugName = typeof(ServiceStackSubscription).Name;

        private DisposeState disposeState = DisposeState.None;

        #region Constructor & Destructor

        internal ServiceStackSubscription(RedisClient client)
        {
            Trace.WriteLine("ENTER: Constructing {0} ...".FormatInvariant(this.debugName));

            this.client = client;

            this.subscription = this.client.CreateSubscription();

            this.subscription.OnMessage = this.OnMessage;
            this.subscription.OnSubscribe = this.OnSubscribe;
            this.subscription.OnUnSubscribe = this.OnUnsubscribe;

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.debugName));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ServiceStackSubscription"/> class.
        /// </summary>
        ~ServiceStackSubscription()
        {
            Trace.WriteLine("ENTER: Destructing {0} ...".FormatInvariant(this.debugName));

            this.Dispose();

            Trace.WriteLine("EXIT: {0} destructed.".FormatInvariant(this.debugName));
        }

        #endregion Constructor & Destructor

        #region IRedisMessageReceiver Members

        /// <inheritdoc />
        public event EventHandler<RedisMessageEventArgs> MessageReceived;

        /// <inheritdoc />
        public IEnumerable<string> ActiveChannels
        {
            get { return this.channels; }
        }

        /// <inheritdoc />
        public void SubscribeToChannels(TimeSpan timeout, IEnumerable<string> channelsToSubscribe)
        {
            if (channelsToSubscribe == null)
            {
                throw new ArgumentNullException("channelsToSubscribe");
            }

            Trace.WriteLine("ENTER: Subscribing to channels {0} ...".FormatInvariant(channelsToSubscribe.ToString(",")));

            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout", timeout, "Value must be positive.");
            }

            if (this.disposeState != DisposeState.None)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            List<string> channelsToSubscribe1 = channelsToSubscribe
                .Except(this.channels)
                .Distinct()
                .ToList();

            if (channelsToSubscribe1.Count == 0)
            {
                Trace.WriteLine("EXIT: No channels to subscribe.");

                return;
            }

            if (this.channels.Count == 0)
            {
                channelsToSubscribe1.Add(ServiceStackSubscription.UnsubscribeChannel);
            }

            SubscribeOperationState subscribeOperation = new SubscribeOperationState(SubscribeOperation.Subscribe, channelsToSubscribe1);

            this.subscribeOperations.Add(subscribeOperation);

            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    this.subscription.SubscribeToChannels(channelsToSubscribe1.ToArray());
                }
                catch (Exception ex)
                {
                    if (ex.IsCritical())
                    {
                        throw;
                    }

                    Trace.TraceWarning(ex.ToString());
                }
            });

            if (timeout == TimeSpan.MaxValue)
            {
                subscribeOperation.Blocker.Wait();
            }
            else if (!subscribeOperation.Blocker.Wait(timeout))
            {
                throw new TimeoutException("Subscribe to channels '{0}' timed out after {1}.".FormatInvariant(channelsToSubscribe.ToString(","), timeout));
            }

            Trace.WriteLine("EXIT: Subscribed to channels {0}.".FormatInvariant(channelsToSubscribe.ToString(",")));
        }

        /// <inheritdoc />
        public bool UnsubscribeFromChannels(TimeSpan timeout, IEnumerable<string> channelsToUnsubscribe)
        {
            if (channelsToUnsubscribe == null)
            {
                throw new ArgumentNullException("channelsToUnsubscribe");
            }

            string channelsDebugString = channelsToUnsubscribe.ToString(",");

            Trace.WriteLine("ENTER: Unsubscribing from channels {0} ...".FormatInvariant(channelsDebugString));

            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout", timeout, "Value must be positive.");
            }

            if (this.disposeState != DisposeState.None)
            {
                Trace.Flush();

                throw new ObjectDisposedException(this.debugName);
            }

            List<string> channelsToUnsubscribe1 = channelsToUnsubscribe.Where(c => this.channels.Contains(c)).Distinct().ToList();

            if (channelsToUnsubscribe1.Count == 0)
            {
                Trace.WriteLine("EXIT: No channels to unsubscribe from.");

                return true;
            }

            if (channelsToUnsubscribe1.Count == this.channels.Count)
            {
                channelsToUnsubscribe1.Add(UnsubscribeChannel);
            }

            SubscribeOperationState subscribeOperation = new SubscribeOperationState(SubscribeOperation.Unsubscribe, channelsToUnsubscribe);

            this.subscribeOperations.Add(subscribeOperation);

            using (RedisClient client2 = new RedisClient(this.client.Host))
            {
                client2.PublishMessage(ServiceStackSubscription.UnsubscribeChannel, RedisConverter.ToString(channelsToUnsubscribe));
            }

            if (timeout == TimeSpan.MaxValue)
            {
                subscribeOperation.Blocker.Wait();

                Trace.WriteLine("EXIT: Unsubscribed from channels {0}.".FormatInvariant(channelsDebugString));

                return true;
            }

            if (subscribeOperation.Blocker.Wait(timeout))
            {
                Trace.WriteLine("EXIT: Unsubscribed from channels {0}.".FormatInvariant(channelsDebugString));

                return true;
            }
            else
            {
                Trace.WriteLine("EXIT: Failed to unsubscribe from channels {0} within the specified timeout {1}.".FormatInvariant(channelsDebugString, timeout));

                return false;
            }
        }

        #endregion IRedisMessageReceiver Members

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            Trace.WriteLine("ENTER: Disposing {0} ...".FormatInvariant(this.debugName));

            this.disposeState = DisposeState.Disposing;

            if (this.subscription != null)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        this.subscription.Dispose();
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsCritical())
                        {
                            throw;
                        }
                    }
                });
            }

            if (this.client != null)
            {
                this.client.Dispose();
            }

            GC.SuppressFinalize(this);

            this.disposeState = DisposeState.Disposed;

            Trace.WriteLine("EXIT: {0} disposed.".FormatInvariant(this.debugName));
        }

        #endregion IDisposable Members

        #region Subscription Events

        private void OnSubscribe(string channel)
        {
            Trace.WriteLine("Subscribed to channel '{0}'.".FormatInvariant(channel));

            if (channel != UnsubscribeChannel)
            {
                this.channels.Add(channel);
            }

            this.OnChannelStateChanged(SubscribeOperation.Subscribe, channel);
        }

        private void OnMessage(string channel, string message)
        {
            Trace.WriteLine("ENTER: Receiving message '{0}' on channel '{1}' ...".FormatInvariant(message, channel));

            if (channel == ServiceStackSubscription.UnsubscribeChannel)
            {
                if (string.IsNullOrEmpty(message))
                {
                    this.subscription.UnSubscribeFromAllChannels();
                }
                else
                {
                    string[] channelToUnsubscribe = RedisConverter.ParseCollection<string>(message).ToArray();

                    this.subscription.UnSubscribeFromChannels(channelToUnsubscribe);
                }
            }
            else
            {
                EventHandler<RedisMessageEventArgs> messageReceivedEventHandler = this.MessageReceived;

                if (messageReceivedEventHandler != null)
                {
                    messageReceivedEventHandler(this, new RedisMessageEventArgs(channel, message));
                }
            }

            Trace.WriteLine("EXIT: Message '{0}' received on channel '{1}'.".FormatInvariant(message, channel));
        }

        private void OnUnsubscribe(string channel)
        {
            Trace.WriteLine("Unsubscribed from channel '{0}'.".FormatInvariant(channel));

            if (channel != UnsubscribeChannel)
            {
                this.channels.Remove(channel);
            }

            this.OnChannelStateChanged(SubscribeOperation.Unsubscribe, channel);
        }

        #endregion Subscription Events

        private void OnChannelStateChanged(SubscribeOperation operation, string channel)
        {
            List<SubscribeOperationState> completedOperations = new List<SubscribeOperationState>();

            foreach (SubscribeOperationState operationState in this.subscribeOperations.Where(o => o.Operation == operation))
            {
                operationState.Channels.Remove(channel);

                if (operationState.Channels.Count == 0)
                {
                    completedOperations.Add(operationState);
                }
            }

            foreach (SubscribeOperationState completedOperation in completedOperations)
            {
                completedOperation.Blocker.Set();

                this.subscribeOperations.Remove(completedOperation);
            }
        }
    }
}