using System;
using System.Diagnostics;
using Radoslav.Redis;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorsMessageBus"/> for monitoring with WPF application.
    /// </summary>
    public sealed class RedisTaskProcessorMonitoringMessageBusSender : ITaskProcessorMessageBusSender
    {
        private readonly IRedisProvider provider;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskProcessorsMonitoringMessageBus"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> is null.</exception>
        public RedisTaskProcessorMonitoringMessageBusSender(IRedisProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            this.provider = provider;
        }

        #endregion Constructor

        #region ITaskProcessorMessageBusSender Members

        /// <inheritdoc />
        public void NotifyStateChanged(Guid taskProcessorId, Model.TaskProcessorState state)
        {
            Trace.WriteLine("ENTER: Notifying task processor '{0}' state changed to {1} ...".FormatInvariant(taskProcessorId, state));

            this.provider.PublishMessage(RedisMonitoringMessageBusChannels.TaskProcessorStateChannel, RedisConverter.ToString(taskProcessorId, state));

            Trace.WriteLine("EXIT: Task processor '{0}' state changed to {1} notified.".FormatInvariant(taskProcessorId, state));
        }

        void ITaskProcessorMessageBusSender.NotifyMasterModeChangeRequest(Guid taskProcessorId, bool isMaster)
        {
            /* Do nothing. */
        }

        void ITaskProcessorMessageBusSender.NotifyMasterModeChanged(Guid taskProcessorId, bool isMaster, MasterModeChangeReason reason)
        {
            /* Do nothing. */
        }

        /// <inheritdoc />
        void ITaskProcessorMessageBusSender.NotifyStopRequested(Guid taskProcessorId)
        {
        }

        /// <inheritdoc />
        void ITaskProcessorMessageBusSender.NotifyPerformanceMonitoring(TimeSpan refreshInterval)
        {
            /* Do nothing. */
        }

        /// <inheritdoc />
        public void NotifyPerformanceReport(TaskProcessorPerformanceReport performanceInfo)
        {
            if (performanceInfo == null)
            {
                throw new ArgumentNullException("performanceInfo");
            }

            Trace.WriteLine("ENTER: Notifying task processor '{0}' performance report ...".FormatInvariant(performanceInfo.TaskProcessorId));

            this.provider.PublishMessage(RedisMonitoringMessageBusChannels.PerformanceReportChannel, RedisMessageBusReceiverBase.SerializeTaskProcessorPerformanceInfo(performanceInfo));

            Trace.WriteLine("EXIT: Task processor '{0}' performance report notified.".FormatInvariant(performanceInfo.TaskProcessorId));
        }

        void ITaskProcessorMessageBusSender.NotifyConfigurationChanged(Guid taskProcessorId)
        {
            /* Do nothing. */
        }

        #endregion ITaskProcessorMessageBusSender Members
    }
}