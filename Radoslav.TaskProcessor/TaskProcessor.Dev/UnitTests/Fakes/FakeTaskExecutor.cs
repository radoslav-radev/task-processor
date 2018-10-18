using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed partial class FakeTaskExecutor : MockObject
    {
        private readonly Dictionary<Guid, TaskPriority> activeTasks = new Dictionary<Guid, TaskPriority>();

        internal IReadOnlyDictionary<Guid, TaskPriority> ActiveTasks
        {
            get { return this.activeTasks; }
        }

        internal void CompleteTask(Guid taskId, TaskStatus status)
        {
            this.activeTasks.Remove(taskId);

            switch (status)
            {
                case TaskStatus.Canceled:
                    if (this.TaskCanceled != null)
                    {
                        this.TaskCanceled(this, new TaskEventArgs(taskId));
                    }

                    break;

                case TaskStatus.Failed:
                    if (this.TaskFailed != null)
                    {
                        this.TaskFailed(this, new TaskEventArgs(taskId));
                    }

                    break;

                case TaskStatus.Success:
                    if (this.TaskCompleted != null)
                    {
                        this.TaskCompleted(this, new TaskCompletedEventArgs(taskId, DateTime.UtcNow, TimeSpan.FromSeconds(1)));
                    }

                    break;
            }
        }
    }
}