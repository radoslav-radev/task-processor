using System;
using System.Diagnostics.CodeAnalysis;
using Radoslav.Redis;

namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    public sealed class RedisTaskMonitoringMessageBusReceiver : RedisMessageBusReceiverBase, ITaskMessageBusReceiver
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskMonitoringMessageBus"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> is null.</exception>
        public RedisTaskMonitoringMessageBusReceiver(IRedisProvider provider)
            : base(provider)
        {
            this.AddChannelMapping(MessageBusChannel.TaskSubmitted, RedisMonitoringMessageBusChannels.TaskRequestedChannel);
            this.AddChannelMapping(MessageBusChannel.TaskProgress, RedisMonitoringMessageBusChannels.TaskProgressChannel);
            this.AddChannelMapping(MessageBusChannel.TaskCancelCompleted, RedisMonitoringMessageBusChannels.TaskCancelCompletedChannel);
            this.AddChannelMapping(MessageBusChannel.TaskFailed, RedisMonitoringMessageBusChannels.TaskFailedChannel);
            this.AddChannelMapping(MessageBusChannel.TaskCompleted, RedisMonitoringMessageBusChannels.TaskCompletedChannel);

            this.SubscribeTimeout = TimeSpan.FromSeconds(5);
        }

        #endregion Constructor

        #region ITaskMessageBusReceiver Members

        /// <inheritdoc />
        public event EventHandler<TaskEventArgs> TaskSubmitted;

        /// <inheritdoc />
        public event EventHandler<TaskProgressEventArgs> TaskProgress;

        /// <inheritdoc />
        public event EventHandler<TaskEventEventArgs> TaskCancelCompleted;

        /// <inheritdoc />
        public event EventHandler<TaskEventEventArgs> TaskFailed;

        /// <inheritdoc />
        public event EventHandler<TaskCompletedEventArgs> TaskCompleted;

        /// <inheritdoc />
        event EventHandler<TaskAssignedEventArgs> ITaskMessageBusReceiver.TaskAssigned
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
        event EventHandler<TaskStartedEventArgs> ITaskMessageBusReceiver.TaskStarted
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
        event EventHandler<TaskEventEventArgs> ITaskMessageBusReceiver.TaskCancelRequested
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

        #endregion ITaskMessageBusReceiver Members

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
                case RedisMonitoringMessageBusChannels.TaskRequestedChannel:
                    this.RaiseEventFromMessage<TaskEventArgs, Guid>(e, this.TaskSubmitted, taskId => new TaskEventArgs(taskId));
                    break;

                case RedisMonitoringMessageBusChannels.TaskProgressChannel:
                    this.RaiseEventFromMessage<TaskProgressEventArgs, Guid, double>(e, this.TaskProgress, (taskId, percentage) => new TaskProgressEventArgs(taskId, percentage));
                    break;

                case RedisMonitoringMessageBusChannels.TaskCancelCompletedChannel:
                    this.RaiseEventFromMessage<TaskEventEventArgs, Guid, DateTime>(e, this.TaskCancelCompleted, (taskId, timestampUtc) => new TaskEventEventArgs(taskId, timestampUtc));
                    break;

                case RedisMonitoringMessageBusChannels.TaskFailedChannel:
                    this.RaiseEventFromMessage<TaskEventEventArgs, Guid, DateTime>(e, this.TaskFailed, (taskId, timestampUtc) => new TaskEventEventArgs(taskId, timestampUtc));
                    break;

                case RedisMonitoringMessageBusChannels.TaskCompletedChannel:
                    this.RaiseEventFromMessage<TaskCompletedEventArgs, Guid, DateTime, TimeSpan>(e, this.TaskCompleted, (taskId, timestampUtc, totalCpuTime) => new TaskCompletedEventArgs(taskId, timestampUtc, totalCpuTime));
                    break;
            }
        }

        #endregion Receive Messages
    }
}