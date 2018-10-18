using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Radoslav.Redis;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorMessageBusReceiver"/> that uses Redis for storage and communication.
    /// </summary>
    public sealed class RedisTaskProcessorMessageBusReceiver : RedisMessageBusReceiverBase, ITaskProcessorMessageBusReceiver
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskProcessorMessageBusReceiver"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> is null.</exception>
        public RedisTaskProcessorMessageBusReceiver(IRedisProvider provider)
            : base(provider)
        {
            this.AddChannelMapping(MessageBusChannel.MasterModeChangeRequest, RedisTaskProcessorChannels.MasterModeChangeRequestChannel);
            this.AddChannelMapping(MessageBusChannel.MasterModeChanged, RedisTaskProcessorChannels.MasterModeChangedChannel);
            this.AddChannelMapping(MessageBusChannel.StopTaskProcessor, RedisTaskProcessorChannels.StopTaskProcessorChannel);
            this.AddChannelMapping(MessageBusChannel.PerformanceMonitoringRequest, RedisTaskProcessorChannels.PerformanceMonitoringChannel);
            this.AddChannelMapping(MessageBusChannel.ConfigurationChanged, RedisTaskProcessorChannels.ConfigurationChangedChannel);
        }

        #endregion Constructor

        #region ITaskProcessorMessageBusReceiver Members

        /// <inheritdoc />
        public event EventHandler<MasterModeChangeEventArgs> MasterModeChangeRequested;

        /// <inheritdoc />
        public event EventHandler<MasterModeChangeEventArgs> MasterModeChanged;

        /// <inheritdoc />
        public event EventHandler<TaskProcessorEventArgs> ConfigurationChanged;

        /// <inheritdoc />
        public event EventHandler<TaskProcessorEventArgs> StopRequested;

        /// <inheritdoc />
        public event EventHandler<PerformanceMonitoringEventArgs> PerformanceMonitoringRequested;

        /// <inheritdoc />
        event EventHandler<TaskProcessorStateEventArgs> ITaskProcessorMessageBusReceiver.StateChanged
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
        event EventHandler<TaskProcessorPerformanceEventArgs> ITaskProcessorMessageBusReceiver.PerformanceReportReceived
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
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = SuppressMessages.ValueIsNeverNull)]
        protected override void OnMessageReceived(object sender, RedisMessageEventArgs e)
        {
            Trace.WriteLine("ENTER: Receiving message '{0}' on channel '{1}' ...".FormatInvariant(e.Message, e.Channel));

            switch (e.Channel)
            {
                case RedisTaskProcessorChannels.PerformanceMonitoringChannel:
                    this.RaiseEventFromMessage<PerformanceMonitoringEventArgs, TimeSpan>(e, this.PerformanceMonitoringRequested, refreshInterval => new PerformanceMonitoringEventArgs(refreshInterval));
                    break;

                case RedisTaskProcessorChannels.StopTaskProcessorChannel:
                    this.RaiseEventFromMessage<TaskProcessorEventArgs, Guid>(e, this.StopRequested, taskProcessorId => new TaskProcessorEventArgs(taskProcessorId));
                    break;

                case RedisTaskProcessorChannels.MasterModeChangeRequestChannel:
                    this.RaiseEventFromMessage<MasterModeChangeEventArgs, Guid, bool>(e, this.MasterModeChangeRequested, (taskProcessorId, isMaster) => new MasterModeChangeEventArgs(taskProcessorId, isMaster, MasterModeChangeReason.Explicit));
                    break;

                case RedisTaskProcessorChannels.MasterModeChangedChannel:
                    this.RaiseEventFromMessage<MasterModeChangeEventArgs, Guid, bool, MasterModeChangeReason>(e, this.MasterModeChanged, (taskProcessorId, isMaster, reason) => new MasterModeChangeEventArgs(taskProcessorId, isMaster, reason));
                    break;

                case RedisTaskProcessorChannels.ConfigurationChangedChannel:
                    this.RaiseEventFromMessage<TaskProcessorEventArgs, Guid>(e, this.ConfigurationChanged, taskProcessorId => new TaskProcessorEventArgs(taskProcessorId));
                    break;

                default:
                    Trace.TraceWarning("EXIT: Message '{0}' received on unknown channel '{1}'.".FormatInvariant(e.Message, e.Channel));
                    return;
            }

            Trace.WriteLine("EXIT: Message '{0}' received on channel '{1}'.".FormatInvariant(e.Message, e.Channel));
        }

        #endregion Receive Messages
    }
}