using System;
using System.Diagnostics;
using Radoslav.Redis;

namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskMessageBusSender"/> that uses Redis for storage and communication.
    /// </summary>
    public sealed class RedisTaskMessageBusSender : ITaskMessageBusSender
    {
        private readonly IRedisProvider provider;
        private readonly ITaskProcessorMessageQueue masterCommandsQueue;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskMessageBusSender"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <param name="masterCommandsQueue">The master commands queue where to push the master commands.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> or <paramref name="masterCommandsQueue"/> is null.</exception>
        public RedisTaskMessageBusSender(IRedisProvider provider, ITaskProcessorMessageQueue masterCommandsQueue)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (masterCommandsQueue == null)
            {
                throw new ArgumentNullException(nameof(masterCommandsQueue));
            }

            this.provider = provider;
            this.masterCommandsQueue = masterCommandsQueue;
        }

        #endregion Constructor

        #region ITaskMessageBusSender Members

        /// <inheritdoc />
        public void NotifyTaskSubmitted(Guid taskId, DateTime timestampUtc, bool isPollingQueueTask)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' request ...".FormatInvariant(taskId));

            if (!isPollingQueueTask)
            {
                this.masterCommandsQueue.Push(new TaskSubmittedMasterCommand(taskId));
            }

            Trace.WriteLine("EXIT: Task '{0}' request notified.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public void NotifyTaskAssigned(Guid taskId, Guid taskProcessorId)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' assigned to processor '{1}' ...".FormatInvariant(taskId, taskProcessorId));

            this.provider.PublishMessage(RedisTaskProcessorChannels.TaskAssignedChannel, RedisConverter.ToString(taskProcessorId, taskId));

            Trace.WriteLine("EXIT: Task '{0}' assigned to processor '{1}' notified.".FormatInvariant(taskId, taskProcessorId));
        }

        /// <inheritdoc />
        public void NotifyTaskStarted(Guid taskId, Guid taskProcessorId, DateTime timestampUtc)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' started by processor '{1}' ...".FormatInvariant(taskId, taskProcessorId));

            this.provider.PublishMessage(RedisTaskProcessorChannels.TaskStartedChannel, RedisConverter.ToString(taskId, taskProcessorId, timestampUtc));

            Trace.WriteLine("EXIT: Task '{0}' started by processor '{1}' notified.".FormatInvariant(taskId, taskProcessorId));
        }

        /// <inheritdoc />
        public void NotifyTaskProgress(Guid taskId, double percentage)
        {
            /* Do nothing. */
        }

        /// <inheritdoc />
        public void NotifyTaskCancelRequest(Guid taskId, DateTime timestampUtc)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' canceled  ...".FormatInvariant(taskId));

            this.provider.PublishMessage(RedisTaskProcessorChannels.TaskCanceledChannel, RedisConverter.ToString(taskId, timestampUtc));

            Trace.WriteLine("EXIT: Task '{0}' canceled notified.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public void NotifyTaskCancelCompleted(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' cancel completed ...".FormatInvariant(taskId));

            this.masterCommandsQueue.Push(new TaskCancelCompletedMasterCommand(taskId, taskProcessorId, timestampUtc)
            {
                IsTaskProcessorStopping = isTaskProcessorStopping
            });

            Trace.WriteLine("EXIT: Task '{0}' cancel completed notified.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public void NotifyTaskFailed(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping, string error)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' fail ...".FormatInvariant(taskId));

            if (string.IsNullOrEmpty(error))
            {
                throw new ArgumentNullException(nameof(error));
            }

            this.masterCommandsQueue.Push(new TaskFailedMasterCommand(taskId, taskProcessorId, timestampUtc, error)
            {
                IsTaskProcessorStopping = isTaskProcessorStopping
            });

            Trace.WriteLine("EXIT: Task '{0}' fail notified.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public void NotifyTaskCompleted(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping, TimeSpan totalCpuTime)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' completed successfully ...".FormatInvariant(taskId));

            this.masterCommandsQueue.Push(new TaskCompletedMasterCommand(taskId, taskProcessorId, timestampUtc)
            {
                IsTaskProcessorStopping = isTaskProcessorStopping,
                TotalCpuTime = totalCpuTime
            });

            Trace.WriteLine("EXIT: Task '{0}' completed successfully notified.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public bool IsSupported(Type taskType)
        {
            return true;
        }

        #endregion ITaskMessageBusSender Members
    }
}