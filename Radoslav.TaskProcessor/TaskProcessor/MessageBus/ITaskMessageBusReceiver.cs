using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Basic functionality of a message bus receiver for task events.
    /// </summary>
    public interface ITaskMessageBusReceiver : IMessageBusReceiver
    {
        /// <summary>
        /// A task has been requested by a user.
        /// </summary>
        event EventHandler<TaskEventArgs> TaskSubmitted;

        /// <summary>
        /// A task has been assigned to a task processor.
        /// </summary>
        event EventHandler<TaskAssignedEventArgs> TaskAssigned;

        /// <summary>
        /// A task has been started.
        /// </summary>
        event EventHandler<TaskStartedEventArgs> TaskStarted;

        /// <summary>
        /// A task has progressed.
        /// </summary>
        event EventHandler<TaskProgressEventArgs> TaskProgress;

        /// <summary>
        /// A task has been requested to cancel by a client.
        /// </summary>
        event EventHandler<TaskEventEventArgs> TaskCancelRequested;

        /// <summary>
        /// A task that has been requested to cancel by a client has been really canceled by the task processor currently executing it.
        /// </summary>
        event EventHandler<TaskEventEventArgs> TaskCancelCompleted;

        /// <summary>
        /// A task execution has failed.
        /// </summary>
        event EventHandler<TaskEventEventArgs> TaskFailed;

        /// <summary>
        /// A task has completed successfully.
        /// </summary>
        event EventHandler<TaskCompletedEventArgs> TaskCompleted;
    }
}