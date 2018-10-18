using System;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed partial class FakeScheduledTask : IScheduledTask
    {
        #region IScheduledTask Members

        public Guid Id { get; set; }

        public ITask NextTask { get; internal set; }

        public TaskPriority NextTaskPriority { get; internal set; }

        public ITaskSummary NextTaskSummary { get; internal set; }

        IScheduleDefinition IScheduledTask.Schedule
        {
            get
            {
                return this.schedule;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        public void PrepareNextTask()
        {
            this.RecordMethodCall();

            this.ExecutePredefinedMethod();
        }

        #endregion IScheduledTask Members
    }
}