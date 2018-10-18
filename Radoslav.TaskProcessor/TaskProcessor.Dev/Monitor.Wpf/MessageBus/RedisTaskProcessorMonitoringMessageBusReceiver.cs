using System;
using System.Diagnostics.CodeAnalysis;
using Radoslav.Redis;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorsMessageBus"/> for monitoring with WPF application.
    /// </summary>
    public sealed class RedisTaskProcessorMonitoringMessageBusReceiver : RedisMessageBusReceiverBase, ITaskProcessorMessageBusReceiver
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskProcessorsMonitoringMessageBus"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> is null.</exception>
        public RedisTaskProcessorMonitoringMessageBusReceiver(IRedisProvider provider)
            : base(provider)
        {
            this.AddChannelMapping(MessageBusChannel.TaskProcessorState, RedisMonitoringMessageBusChannels.TaskProcessorStateChannel);
            this.AddChannelMapping(MessageBusChannel.PerformanceReport, RedisMonitoringMessageBusChannels.PerformanceReportChannel);

            this.SubscribeTimeout = TimeSpan.FromSeconds(5);
        }

        #endregion Constructor

        #region ITaskProcessorMessageBusReceiver Members

        /// <inheritdoc />
        public event EventHandler<TaskProcessorStateEventArgs> StateChanged;

        /// <inheritdoc />
        public event EventHandler<TaskProcessorPerformanceEventArgs> PerformanceReportReceived;

        /// <inheritdoc />
        event EventHandler<TaskProcessorEventArgs> ITaskProcessorMessageBusReceiver.ConfigurationChanged
        {
            add
            {
                /* Do nothing. */
            }

            remove
            {
                /* Do nothing. */
            }
        }

        event EventHandler<MasterModeChangeEventArgs> ITaskProcessorMessageBusReceiver.MasterModeChangeRequested
        {
            add
            {
                /* Do nothing. */
            }

            remove
            {
                /* Do nothing. */
            }
        }

        event EventHandler<MasterModeChangeEventArgs> ITaskProcessorMessageBusReceiver.MasterModeChanged
        {
            add
            {
                /* Do nothing. */
            }

            remove
            {
                /* Do nothing. */
            }
        }

        /// <inheritdoc />
        event EventHandler<TaskProcessorEventArgs> ITaskProcessorMessageBusReceiver.StopRequested
        {
            add
            {
                /* Do nothing. */
            }

            remove
            {
                /* Do nothing. */
            }
        }

        /// <inheritdoc />
        event EventHandler<PerformanceMonitoringEventArgs> ITaskProcessorMessageBusReceiver.PerformanceMonitoringRequested
        {
            add
            {
                /* Do nothing. */
            }

            remove
            {
                /* Do nothing. */
            }
        }

        #endregion ITaskProcessorMessageBusReceiver Members

        #region Receive Messages

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This method is actually very simple and there is nothing wrong with it.")]
        protected override void OnMessageReceived(object sender, RedisMessageEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            switch (e.Channel)
            {
                case RedisMonitoringMessageBusChannels.TaskProcessorStateChannel:
                    this.RaiseEventFromMessage<TaskProcessorStateEventArgs, Guid, TaskProcessorState>(e, this.StateChanged, (taskProcessorId, state) => new TaskProcessorStateEventArgs(taskProcessorId, state));
                    break;

                case RedisMonitoringMessageBusChannels.PerformanceReportChannel:
                    this.RaiseEventFromMessage<TaskProcessorPerformanceEventArgs, string>(e, this.PerformanceReportReceived, value => new TaskProcessorPerformanceEventArgs(RedisMessageBusReceiverBase.DeserializeTaskProcessorPerformanceInfo(value)));
                    break;
            }
        }

        #endregion Receive Messages
    }
}