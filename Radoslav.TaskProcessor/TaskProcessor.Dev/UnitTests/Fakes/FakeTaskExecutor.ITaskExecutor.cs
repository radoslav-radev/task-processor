using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal partial class FakeTaskExecutor : ITaskExecutor
    {
        #region ITaskExecutor Members

        public event EventHandler<TaskEventArgs> TaskCanceled;

        public event EventHandler<TaskEventArgs> TaskFailed;

        public event EventHandler<TaskCompletedEventArgs> TaskCompleted;

        public int ActiveTasksCount
        {
            get { return this.activeTasks.Count; }
        }

        public TimeSpan CancelTimeout { get; set; }

        public bool MonitorPerformance { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Test code.")]
        public void StartTask(Guid taskId, TaskPriority priority)
        {
            this.RecordMethodCall(taskId, priority);

            this.activeTasks.Add(taskId, priority);

            try
            {
                this.ExecutePredefinedMethod(taskId, priority);

                this.CompleteTask(taskId, TaskStatus.Success);
            }
            catch
            {
                this.CompleteTask(taskId, TaskStatus.Failed);
            }
        }

        public void CancelTask(Guid taskId)
        {
            this.RecordMethodCall(taskId);

            this.CompleteTask(taskId, TaskStatus.Canceled);
        }

        public IEnumerable<TaskPerformanceReport> GetTasksPerformanceInfo()
        {
            this.RecordMethodCall();

            if (this.HasPredefinedResult<IEnumerable<TaskPerformanceReport>>())
            {
                return this.GetPredefinedResult<IEnumerable<TaskPerformanceReport>>();
            }
            else
            {
                return Enumerable.Empty<TaskPerformanceReport>();
            }
        }

        #endregion ITaskExecutor Members
    }
}