using System;
using System.Collections.Generic;
using System.Diagnostics;
using Radoslav.Redis;

namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    public sealed class RedisTaskMonitoringMessageBusSender : ITaskMessageBusSender
    {
        private readonly IRedisProvider provider;
        private readonly HashSet<Type> supportedTaskTypes = new HashSet<Type>();

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskMonitoringMessageBusSender"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> is null.</exception>
        public RedisTaskMonitoringMessageBusSender(IRedisProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            this.provider = provider;
        }

        #endregion Constructor

        public ICollection<Type> SupportedTaskTypes
        {
            get { return this.supportedTaskTypes; }
        }

        #region ITaskMessageBusSender Members

        /// <inheritdoc />
        public void NotifyTaskSubmitted(Guid taskId, DateTime timestampUtc, bool isPollingQueueTask)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' request ...".FormatInvariant(taskId));

            this.provider.PublishMessage(RedisMonitoringMessageBusChannels.TaskRequestedChannel, RedisConverter.ToString(taskId));

            Trace.WriteLine("EXIT: Task '{0}' request notified.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        void ITaskMessageBusSender.NotifyTaskAssigned(Guid taskId, Guid taskProcessorId)
        {
            /* Do nothing. */
        }

        /// <inheritdoc />
        void ITaskMessageBusSender.NotifyTaskStarted(Guid taskId, Guid taskProcessorId, DateTime timestampUtc)
        {
            /* Do nothing. */
        }

        /// <inheritdoc />
        public void NotifyTaskProgress(Guid taskId, double percentage)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' progress {1}% ...".FormatInvariant(taskId, percentage));

            this.provider.PublishMessage(RedisMonitoringMessageBusChannels.TaskProgressChannel, RedisConverter.ToString(taskId, percentage));

            Trace.WriteLine("EXIT: task '{0}' progress {1}% notified.".FormatInvariant(taskId, percentage));
        }

        /// <inheritdoc />
        void ITaskMessageBusSender.NotifyTaskCancelRequest(Guid taskId, DateTime timestampUtc)
        {
            /* Do nothing. */
        }

        /// <inheritdoc />
        public void NotifyTaskCancelCompleted(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' cancel completed ...".FormatInvariant(taskId));

            this.provider.PublishMessage(RedisMonitoringMessageBusChannels.TaskCancelCompletedChannel, RedisConverter.ToString(taskId, timestampUtc));

            Trace.WriteLine("EXIT: Task '{0}' cancel completed notified.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public void NotifyTaskFailed(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping, string error)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' failed ...".FormatInvariant(taskId));

            this.provider.PublishMessage(RedisMonitoringMessageBusChannels.TaskFailedChannel, RedisConverter.ToString(taskId, timestampUtc));

            Trace.WriteLine("EXIT: Task '{0}' failed notified.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public void NotifyTaskCompleted(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping, TimeSpan totalCpuTime)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' completed ...".FormatInvariant(taskId));

            this.provider.PublishMessage(RedisMonitoringMessageBusChannels.TaskCompletedChannel, RedisConverter.ToString(taskId, timestampUtc, totalCpuTime));

            Trace.WriteLine("EXIT: Task '{0}' completed notified.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public bool IsSupported(Type taskType)
        {
            return this.supportedTaskTypes.IsEmpty() || this.supportedTaskTypes.Contains(taskType);
        }

        #endregion ITaskMessageBusSender Members
    }
}