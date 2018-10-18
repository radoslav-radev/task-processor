using System;
using System.Diagnostics;
using Radoslav.Redis;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorMessageBusSender"/> that uses Redis for storage and communication.
    /// </summary>
    public sealed class RedisTaskProcessorMessageBusSender : ITaskProcessorMessageBusSender
    {
        private readonly IRedisProvider provider;
        private readonly ITaskProcessorMessageQueue masterCommandsQueue;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskProcessorMessageBusSender"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <param name="masterCommandsQueue">The master commands queue where to push the master commands.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> or <paramref name="masterCommandsQueue"/> is null.</exception>
        public RedisTaskProcessorMessageBusSender(IRedisProvider provider, ITaskProcessorMessageQueue masterCommandsQueue)
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

        #region ITaskProcessorMessageBusSender Members

        /// <inheritdoc />
        public void NotifyMasterModeChangeRequest(Guid taskProcessorId, bool isMaster)
        {
            Trace.WriteLine("ENTER: Notifying task processor '{0}' master mode {1} change request ...".FormatInvariant(taskProcessorId, isMaster));

            this.provider.PublishMessage(RedisTaskProcessorChannels.MasterModeChangeRequestChannel, RedisConverter.ToString(taskProcessorId, isMaster));

            Trace.WriteLine("EXIT: Task processor '{0}' master mode {1} change request notified.".FormatInvariant(taskProcessorId, isMaster));
        }

        /// <inheritdoc />
        public void NotifyMasterModeChanged(Guid taskProcessorId, bool isMaster, MasterModeChangeReason reason)
        {
            Trace.WriteLine("ENTER: Notifying task processor '{0}' master mode changed to {1} with reason {2} ...".FormatInvariant(taskProcessorId, isMaster, reason));

            MasterModeChangeEventArgs.ValidateArguments(isMaster, reason);

            this.provider.PublishMessage(RedisTaskProcessorChannels.MasterModeChangedChannel, RedisConverter.ToString(taskProcessorId, isMaster, reason));

            Trace.WriteLine("EXIT: Task processor '{0}' master mode changed to {1} with reason {2} notified.".FormatInvariant(taskProcessorId, isMaster, reason));
        }

        /// <inheritdoc />
        public void NotifyStateChanged(Guid taskProcessorId, TaskProcessorState state)
        {
            Trace.WriteLine("ENTER: Notifying task processor '{0}' state changed to {1} ...".FormatInvariant(taskProcessorId, state));

            if (state == TaskProcessorState.Active)
            {
                this.masterCommandsQueue.Push(new TaskProcessorRegisteredMasterCommand(taskProcessorId));
            }

            Trace.WriteLine("EXIT: Task processor '{0}' state changed to {1} notified.".FormatInvariant(taskProcessorId, state));
        }

        /// <inheritdoc />
        public void NotifyStopRequested(Guid taskProcessorId)
        {
            Trace.WriteLine("ENTER: Notifying task '{0}' processor stop request ...".FormatInvariant(taskProcessorId));

            this.provider.PublishMessage(RedisTaskProcessorChannels.StopTaskProcessorChannel, RedisConverter.ToString(taskProcessorId));

            Trace.WriteLine("EXIT: Task '{0}' processor stop request notified.".FormatInvariant(taskProcessorId));
        }

        /// <inheritdoc />
        public void NotifyPerformanceMonitoring(TimeSpan refreshInterval)
        {
            Trace.WriteLine("ENTER: Notifying task processor performance monitoring ...");

            if (refreshInterval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("refreshInterval", refreshInterval, "Value must be positive.");
            }

            this.provider.PublishMessage(RedisTaskProcessorChannels.PerformanceMonitoringChannel, RedisConverter.ToString(refreshInterval));

            Trace.WriteLine("EXIT: Task processor performance monitoring.");
        }

        /// <inheritdoc />
        public void NotifyPerformanceReport(TaskProcessorPerformanceReport performanceInfo)
        {
            /* Do nothing. */
        }

        /// <inheritdoc />
        public void NotifyConfigurationChanged(Guid taskProcessorId)
        {
            Trace.WriteLine("ENTER: Notifying task processor '{0}' configuration changed ...".FormatInvariant(taskProcessorId));

            this.provider.PublishMessage(RedisTaskProcessorChannels.ConfigurationChangedChannel, RedisConverter.ToString(taskProcessorId));

            this.masterCommandsQueue.Push(new ConfigurationChangedMasterCommand(taskProcessorId));

            Trace.WriteLine("EXIT: Task processor '{0}' configuration changed notified.".FormatInvariant(taskProcessorId));
        }

        #endregion ITaskProcessorMessageBusSender Members
    }
}