using System;
using Radoslav.TaskProcessor.MessageBus;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed partial class FakeTaskMessageBus : FakeMessageBusBase, ITaskMessageBusSender, ITaskMessageBusReceiver, ITaskMessageBus
    {
        internal FakeTaskMessageBus()
        {
        }

        internal FakeTaskMessageBus(FakeMessageQueue masterCommands)
            : base(masterCommands)
        {
        }

        #region ITaskMessageBusReceiver Members

        public event EventHandler<TaskEventArgs> TaskSubmitted;

        public event EventHandler<TaskAssignedEventArgs> TaskAssigned;

        public event EventHandler<TaskStartedEventArgs> TaskStarted;

        public event EventHandler<TaskProgressEventArgs> TaskProgress;

        public event EventHandler<TaskEventEventArgs> TaskCancelRequested;

        public event EventHandler<TaskEventEventArgs> TaskCancelCompleted;

        public event EventHandler<TaskEventEventArgs> TaskFailed;

        public event EventHandler<TaskCompletedEventArgs> TaskCompleted;

        #endregion ITaskMessageBusReceiver Members

        #region ITaskMessageBus Members

        ITaskMessageBusReceiver ITaskMessageBus.Receiver
        {
            get { return this; }
        }

        internal FakeTaskMessageBus Receiver
        {
            get { return this; }
        }

        public ITaskMessageBusSender GetSender(Type taskType)
        {
            return this;
        }

        #endregion ITaskMessageBus Members

        #region ITaskMessageBusSender Members

        public void NotifyTaskSubmitted(Guid taskId, DateTime timestampUtc, bool isPollingQueueTask)
        {
            this.RecordMethodCall(taskId, timestampUtc, isPollingQueueTask);

            if (!isPollingQueueTask)
            {
                this.MasterCommands.Push(new TaskSubmittedMasterCommand(taskId));
            }

            if (this.SubscribedChannels.Contains(MessageBusChannel.TaskSubmitted))
            {
                this.TaskSubmitted(this, new TaskEventArgs(taskId));
            }
        }

        public void NotifyTaskAssigned(Guid taskId, Guid taskProcessorId)
        {
            this.RecordMethodCall(taskId, taskProcessorId);

            if ((this.TaskAssigned != null) && this.SubscribedChannels.Contains(MessageBusChannel.TaskAssigned))
            {
                this.TaskAssigned(this, new TaskAssignedEventArgs(taskId, taskProcessorId));
            }
        }

        public void NotifyTaskStarted(Guid taskId, Guid taskProcessorId, DateTime timestampUtc)
        {
            this.RecordMethodCall(taskId, taskProcessorId, timestampUtc);

            if (this.TaskStarted != null)
            {
                if (this.MasterCommands.ReceiveMessages || this.SubscribedChannels.Contains(MessageBusChannel.TaskStarted))
                {
                    this.TaskStarted(this, new TaskStartedEventArgs(taskId, taskProcessorId, timestampUtc));
                }
            }
        }

        public void NotifyTaskProgress(Guid taskId, double percentage)
        {
            if ((percentage < 0) || (percentage > 100))
            {
                throw new ArgumentOutOfRangeException("percentage", percentage, "Value must be between 0 and 100.");
            }

            this.RecordMethodCall(taskId, percentage);

            if ((this.TaskProgress != null) && this.SubscribedChannels.Contains(MessageBusChannel.TaskProgress))
            {
                this.TaskProgress(this, new TaskProgressEventArgs(taskId, percentage));
            }
        }

        public void NotifyTaskCancelRequest(Guid taskId, DateTime timestampUtc)
        {
            this.RecordMethodCall(taskId, timestampUtc);

            if ((this.TaskCancelRequested != null) && this.SubscribedChannels.Contains(MessageBusChannel.TaskCancelRequest))
            {
                this.TaskCancelRequested(this, new TaskEventEventArgs(taskId, timestampUtc));
            }
        }

        public void NotifyTaskCancelCompleted(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping)
        {
            this.RecordMethodCall(taskId, timestampUtc, taskProcessorId, isTaskProcessorStopping);

            this.MasterCommands.Push(new TaskCancelCompletedMasterCommand(taskId, taskProcessorId, timestampUtc)
            {
                IsTaskProcessorStopping = isTaskProcessorStopping
            });

            if ((this.TaskCancelCompleted != null) && this.SubscribedChannels.Contains(MessageBusChannel.TaskCancelCompleted))
            {
                this.TaskCancelCompleted(this, new TaskEventEventArgs(taskId, timestampUtc));
            }
        }

        public void NotifyTaskFailed(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping, string error)
        {
            this.RecordMethodCall(taskId, timestampUtc, taskProcessorId, isTaskProcessorStopping, error);

            this.MasterCommands.Push(new TaskFailedMasterCommand(taskId, taskProcessorId, timestampUtc, error)
            {
                IsTaskProcessorStopping = isTaskProcessorStopping
            });

            if ((this.TaskFailed != null) && this.SubscribedChannels.Contains(MessageBusChannel.TaskFailed))
            {
                this.TaskFailed(this, new TaskEventEventArgs(taskId, timestampUtc));
            }
        }

        public void NotifyTaskCompleted(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping, TimeSpan totalCpuTime)
        {
            this.RecordMethodCall(taskId, timestampUtc, taskProcessorId, isTaskProcessorStopping, totalCpuTime);

            this.MasterCommands.Push(new TaskCompletedMasterCommand(taskId, taskProcessorId, timestampUtc)
            {
                IsTaskProcessorStopping = isTaskProcessorStopping,
                TotalCpuTime = totalCpuTime
            });

            if ((this.TaskCompleted != null) && this.SubscribedChannels.Contains(MessageBusChannel.TaskCompleted))
            {
                this.TaskCompleted(this, new TaskCompletedEventArgs(taskId, timestampUtc, totalCpuTime));
            }
        }

        public bool IsSupported(Type taskType)
        {
            this.RecordMethodCall(taskType);

            return this.GetPredefinedResult<bool>(taskType);
        }

        #endregion ITaskMessageBusSender Members
    }
}