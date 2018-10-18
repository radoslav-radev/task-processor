using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Radoslav.Redis;

namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskMessageBusReceiver"/> that uses Redis for storage and communication.
    /// </summary>
    public sealed class RedisTaskMessageBusReceiver : RedisMessageBusReceiverBase, ITaskMessageBusReceiver
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskMessageBusReceiver"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> is null.</exception>
        public RedisTaskMessageBusReceiver(IRedisProvider provider)
            : base(provider)
        {
            this.AddChannelMapping(MessageBusChannel.TaskStarted, RedisTaskProcessorChannels.TaskStartedChannel);
            this.AddChannelMapping(MessageBusChannel.TaskAssigned, RedisTaskProcessorChannels.TaskAssignedChannel);
            this.AddChannelMapping(MessageBusChannel.TaskCancelRequest, RedisTaskProcessorChannels.TaskCanceledChannel);
        }

        #endregion Constructor

        #region ITasksMessageBus Members

        /// <inheritdoc />
        public event EventHandler<TaskAssignedEventArgs> TaskAssigned;

        /// <inheritdoc />
        public event EventHandler<TaskStartedEventArgs> TaskStarted;

        /// <inheritdoc />
        public event EventHandler<TaskEventEventArgs> TaskCancelRequested;

        /// <inheritdoc />
        event EventHandler<TaskEventEventArgs> ITaskMessageBusReceiver.TaskCancelCompleted
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
        event EventHandler<TaskEventEventArgs> ITaskMessageBusReceiver.TaskFailed
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
        event EventHandler<TaskCompletedEventArgs> ITaskMessageBusReceiver.TaskCompleted
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
        event EventHandler<TaskProgressEventArgs> ITaskMessageBusReceiver.TaskProgress
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
        event EventHandler<TaskEventArgs> ITaskMessageBusReceiver.TaskSubmitted
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

        #endregion ITasksMessageBus Members

        #region Receive Messages

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = SuppressMessages.ValueIsNeverNull)]
        protected override void OnMessageReceived(object sender, RedisMessageEventArgs e)
        {
            Trace.WriteLine("ENTER: Receiving message '{0}' on channel '{1}' ...".FormatInvariant(e.Message, e.Channel));

            switch (e.Channel)
            {
                case RedisTaskProcessorChannels.TaskAssignedChannel:
                    this.RaiseEventFromMessage<TaskAssignedEventArgs, Guid, Guid>(e, this.TaskAssigned, (taskProcessorId, taskId) => new TaskAssignedEventArgs(taskId, taskProcessorId));
                    break;

                case RedisTaskProcessorChannels.TaskStartedChannel:
                    this.RaiseEventFromMessage<TaskStartedEventArgs, Guid, Guid, DateTime>(e, this.TaskStarted, (taskId, taskProcessorId, timestampUtc) => new TaskStartedEventArgs(taskId, taskProcessorId, timestampUtc));
                    break;

                case RedisTaskProcessorChannels.TaskCanceledChannel:
                    this.RaiseEventFromMessage<TaskEventEventArgs, Guid, DateTime, object>(e, this.TaskCancelRequested, (taskId, timestampUtc, v3) => new TaskEventEventArgs(taskId, timestampUtc));
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