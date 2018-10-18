using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// An composite implementation of the <see cref="ITaskMessageBusReceiver"/> interface.
    /// </summary>
    internal sealed class CompositeTaskMessageBusReceiver : CompositeMessageBusReceiver<ITaskMessageBusReceiver>, ITaskMessageBusReceiver
    {
        #region ITaskMessageBusReceiver Members

        /// <inheritdoc />
        event EventHandler<TaskEventArgs> ITaskMessageBusReceiver.TaskSubmitted
        {
            add
            {
                this.ForEach(mb => mb.TaskSubmitted += value);
            }

            remove
            {
                this.ForEach(mb => mb.TaskSubmitted -= value);
            }
        }

        /// <inheritdoc />
        event EventHandler<TaskAssignedEventArgs> ITaskMessageBusReceiver.TaskAssigned
        {
            add
            {
                this.ForEach(mb => mb.TaskAssigned += value);
            }

            remove
            {
                this.ForEach(mb => mb.TaskAssigned -= value);
            }
        }

        /// <inheritdoc />
        event EventHandler<TaskStartedEventArgs> ITaskMessageBusReceiver.TaskStarted
        {
            add
            {
                this.ForEach(mb => mb.TaskStarted += value);
            }

            remove
            {
                this.ForEach(mb => mb.TaskStarted -= value);
            }
        }

        /// <inheritdoc />
        event EventHandler<TaskProgressEventArgs> ITaskMessageBusReceiver.TaskProgress
        {
            add
            {
                this.ForEach(mb => mb.TaskProgress += value);
            }

            remove
            {
                this.ForEach(mb => mb.TaskProgress -= value);
            }
        }

        /// <inheritdoc />
        event EventHandler<TaskEventEventArgs> ITaskMessageBusReceiver.TaskCancelRequested
        {
            add
            {
                this.ForEach(mb => mb.TaskCancelRequested += value);
            }

            remove
            {
                this.ForEach(mb => mb.TaskCancelRequested -= value);
            }
        }

        /// <inheritdoc />
        event EventHandler<TaskEventEventArgs> ITaskMessageBusReceiver.TaskCancelCompleted
        {
            add
            {
                this.ForEach(mb => mb.TaskCancelCompleted += value);
            }

            remove
            {
                this.ForEach(mb => mb.TaskCancelCompleted -= value);
            }
        }

        /// <inheritdoc />
        event EventHandler<TaskEventEventArgs> ITaskMessageBusReceiver.TaskFailed
        {
            add
            {
                this.ForEach(mb => mb.TaskFailed += value);
            }

            remove
            {
                this.ForEach(mb => mb.TaskFailed -= value);
            }
        }

        /// <inheritdoc />
        event EventHandler<TaskCompletedEventArgs> ITaskMessageBusReceiver.TaskCompleted
        {
            add
            {
                this.ForEach(mb => mb.TaskCompleted += value);
            }

            remove
            {
                this.ForEach(mb => mb.TaskCompleted -= value);
            }
        }

        #endregion ITaskMessageBusReceiver Members
    }
}