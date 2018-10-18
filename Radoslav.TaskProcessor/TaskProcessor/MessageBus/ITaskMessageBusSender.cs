using System;
using System.Collections.Generic;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Basic functionality of a sender of task messages.
    /// </summary>
    public interface ITaskMessageBusSender
    {
        /// <summary>
        /// Checks if the sender supports tasks of certain type.
        /// </summary>
        /// <param name="taskType">The type of the tasks to check.</param>
        /// <returns>True if the sender supports tasks of the specified type; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType"/> is null.</exception>
        bool IsSupported(Type taskType);

        /// <summary>
        /// Notifies that a task has been requested.
        /// </summary>
        /// <param name="taskId">The ID of the requested task.</param>
        /// <param name="timestampUtc">When the task has been submitted, in UTC.</param>
        /// <param name="isPollingQueueTask">Whether the requested task is a polling queue task.</param>
        /// <remarks>
        /// <para>Event <see cref="ITaskMessageBusReceiver.TaskSubmitted"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.TaskRequested.</para>
        /// <para>Also, a <see cref="TaskSubmittedMasterCommand"/> will be added to the <see cref="ITaskProcessorMessageBus.MasterCommands"/> queue.</para>
        /// </remarks>
        void NotifyTaskSubmitted(Guid taskId, DateTime timestampUtc, bool isPollingQueueTask);

        /// <summary>
        /// Notifies that a task has been assigned to a task processor by the master task processor.
        /// </summary>
        /// <param name="taskId">The ID of the assigned task.</param>
        /// <param name="taskProcessorId">The ID of the task processor that the task has been assigned to.</param>
        /// <remarks>Event <see cref="ITaskMessageBusReceiver.TaskAssigned"/> will be raised for all subscribers who have
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.TaskAssigned.</remarks>
        void NotifyTaskAssigned(Guid taskId, Guid taskProcessorId);

        /// <summary>
        /// Notifies that a task been started by a task processor.
        /// </summary>
        /// <param name="taskId">The ID of the task that has been started.</param>
        /// <param name="taskProcessorId">The ID of the task processor that has started the task.</param>
        /// <param name="timestampUtc">When the task has been started, in UTC.</param>
        /// <remarks>Event <see cref="ITaskMessageBusReceiver.TaskStarted"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.TaskStarted.</remarks>
        void NotifyTaskStarted(Guid taskId, Guid taskProcessorId, DateTime timestampUtc);

        /// <summary>
        /// Notifies that a task has progressed.
        /// </summary>
        /// <param name="taskId">The ID of the task that has progressed.</param>
        /// <param name="percentage">The percent of completion.</param>
        /// <remarks>Event <see cref="ITaskMessageBusReceiver.TaskProgress"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.TaskProgress.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="percentage"/> is less than 0 or greater than 100.</exception>
        void NotifyTaskProgress(Guid taskId, double percentage);

        /// <summary>
        /// Notifies that a client has requested the task processor to cancel a task.
        /// </summary>
        /// <param name="taskId">The ID of the task to be canceled.</param>
        /// <param name="timestampUtc">When the task has been requested to cancel, in UTC.</param>
        /// <remarks>
        /// <para>Event <see cref="ITaskMessageBusReceiver.TaskCancelRequested"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.TaskCancelRequest.</para>
        /// </remarks>
        void NotifyTaskCancelRequest(Guid taskId, DateTime timestampUtc);

        /// <summary>
        /// Notifies that a canceled task execution has been aborted by the task processor executing the task.
        /// </summary>
        /// <remarks>
        /// <para>Event <see cref="ITaskMessageBusReceiver.TaskCancelCompleted"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.TaskCancelCompleted.</para>
        /// <para>Also, a <see cref="TaskCancelCompletedMasterCommand"/> will be added to the <see cref="ITaskProcessorMessageBus.MasterCommands"/> queue.</para>
        /// </remarks>
        /// <param name="taskId">The ID of the task that has been canceled.</param>
        /// <param name="timestampUtc">When the task execution was aborted, in UTC.</param>
        /// <param name="taskProcessorId">The ID of the task processor that was executing the task.</param>
        /// <param name="isTaskProcessorStopping">Whether the task processor that was executing the task is stopping.</param>
        void NotifyTaskCancelCompleted(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping);

        /// <summary>
        /// Notifies that an error occurred during a task execution.
        /// </summary>
        /// <remarks>
        /// <para>Event <see cref="ITaskMessageBusReceiver.TaskFailed"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.TaskFailed.</para>
        /// <para>Also, a <see cref="TaskFailedMasterCommand"/> will be added to the <see cref="ITaskProcessorMessageBus.MasterCommands"/> queue.</para>
        /// </remarks>
        /// <param name="taskId">The ID of the task that has failed.</param>
        /// <param name="timestampUtc">When the task has failed, in UTC.</param>
        /// <param name="taskProcessorId">The ID of the task processor that was executing the task.</param>
        /// <param name="isTaskProcessorStopping">Whether the task processor that was executing the task is stopping.</param>
        /// <param name="error">The error that occurred during the task execution.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="error"/> is null or empty.</exception>
        void NotifyTaskFailed(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping, string error);

        /// <summary>
        /// Notifies that a task execution has completed successfully.
        /// </summary>
        /// <remarks>
        /// <para>Event <see cref="ITaskMessageBusReceiver.TaskCompleted"/> will be raised for all subscribers who have called
        /// <see cref="IMessageBusReceiver.SubscribeForChannels(IEnumerable{MessageBusChannel})"/> with parameter <see cref="MessageBusChannel"/>.TaskCompleted.</para>
        /// <para>Also, a <see cref="TaskCompletedMasterCommand"/> will be added to the <see cref="ITaskProcessorMessageBus.MasterCommands"/> queue.</para>
        /// </remarks>
        /// <param name="taskId">The ID of the task that has completed.</param>
        /// <param name="timestampUtc">When the task has completed, in UTC.</param>
        /// <param name="taskProcessorId">The ID of the task processor that has completed the task.</param>
        /// <param name="isTaskProcessorStopping">Whether the task processor that has completed the task is stopping.</param>
        /// <param name="totalCpuTime">The total processor time used during task execution.</param>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="totalCpuTime"/> is less than <see cref="TimeSpan.Zero"/>.</exception>
        void NotifyTaskCompleted(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping, TimeSpan totalCpuTime);
    }
}