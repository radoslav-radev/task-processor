using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Basic functionality of a sender of task processors events.
    /// </summary>
    public interface ITaskProcessorMessageBusSender
    {
        /// <summary>
        /// Notifies that a task processor state has changed.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor whose state has changed.</param>
        /// <param name="state">The new state of the task processor.</param>
        /// <remarks>
        /// <para>Event <see cref="ITaskProcessorMessageBusReceiver.StateChanged"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/>
        /// with parameter <see cref="MessageBusChannel"/>.TaskProcessorState.</para>.
        /// <para>Also, if the state is <see cref="TaskProcessorState.Active"/>, i.e. the task processor has been registered,
        /// a <see cref="TaskProcessorRegisteredMasterCommand"/> will be added to the <see cref="ITaskProcessorMessageBus.MasterCommands"/> queue.</para>
        /// </remarks>
        void NotifyStateChanged(Guid taskProcessorId, TaskProcessorState state);

        /// <summary>
        /// Notifies a task processor to become a master or a slave.
        /// </summary>
        /// <remarks>Event <see cref="ITaskProcessorMessageBusReceiver.MasterModeChangeRequested"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.MasterModeChangeRequested.</remarks>
        /// <param name="taskProcessorId">The ID of the task processor.</param>
        /// <param name="isMaster">Whether the task processor should become a master, or a slave.</param>
        void NotifyMasterModeChangeRequest(Guid taskProcessorId, bool isMaster);

        /// <summary>
        /// Notifies that a task processor has become a master or a slave.
        /// </summary>
        /// <remarks>Event <see cref="ITaskProcessorMessageBusReceiver.MasterModeChanged"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.MasterModeChanged.</remarks>
        /// <param name="taskProcessorId">The task processor who has become a master or a slave.</param>
        /// <param name="isMaster"><c>true</c> if the task processor has become master; <c>false</c> if the task processor has become slave.</param>
        /// <param name="reason">The reason why the task processor has become a master or a slave.</param>
        void NotifyMasterModeChanged(Guid taskProcessorId, bool isMaster, MasterModeChangeReason reason);

        /// <summary>
        /// Notifies a task processor to stop.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor that should stop.</param>
        /// <remarks>Event <see cref="ITaskProcessorMessageBusReceiver.StateChanged"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.StopTaskProcessor.</remarks>
        void NotifyStopRequested(Guid taskProcessorId);

        /// <summary>
        /// Notifies a task processor that a performance monitoring is started.
        /// </summary>
        /// <remarks>Event <see cref="ITaskProcessorMessageBusReceiver.PerformanceMonitoringRequested"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.PerformanceMonitoringRequest.</remarks>
        /// <param name="refreshInterval">The time interval between two performance report messages sent by the task processor.</param>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="refreshInterval"/> is less than or equal to <see cref="TimeSpan.Zero"/>.</exception>
        void NotifyPerformanceMonitoring(TimeSpan refreshInterval);

        /// <summary>
        /// Sends a task processor performance report to interested monitoring tools.
        /// </summary>
        /// <remarks>Event <see cref="ITaskProcessorMessageBusReceiver.PerformanceReportReceived"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.PerformanceReport.</remarks>
        /// <param name="performanceInfo">The performance metrics of the task processor and all tasks currently executed by it.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="performanceInfo"/> is null.</exception>
        void NotifyPerformanceReport(TaskProcessorPerformanceReport performanceInfo);

        /// <summary>
        /// Notifies that a task processor configuration has been changed.
        /// </summary>
        /// <remarks>Event <see cref="ITaskProcessorMessageBusReceiver.ConfigurationChanged"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.ConfigurationChanged.</remarks>
        /// <param name="taskProcessorId">The ID of the task processor whose configuration has been changed.</param>
        void NotifyConfigurationChanged(Guid taskProcessorId);
    }
}