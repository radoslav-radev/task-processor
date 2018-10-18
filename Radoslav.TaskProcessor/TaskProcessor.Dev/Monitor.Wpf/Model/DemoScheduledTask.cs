using System;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.Model
{
    [Serializable]
    public sealed class DemoScheduledTask : IScheduledTask
    {
        private readonly Random randomizer = new Random();

        private DemoTask nextTask;
        private ITaskSummary nextSummary;
        private TaskPriority nextPriority;

        public int MinDurationInSeconds { get; set; }

        public int MaxDurationInSeconds { get; set; }

        public DemoRecurrenceDefinition RecurrenceDefinition { get; set; }

        #region IScheduledTask Members

        public Guid Id { get; set; }

        public ITask NextTask
        {
            get { return this.nextTask; }
        }

        public TaskPriority NextTaskPriority
        {
            get { return this.nextPriority; }
        }

        public ITaskSummary NextTaskSummary
        {
            get { return this.nextSummary; }
        }

        IScheduleDefinition IScheduledTask.Schedule
        {
            get
            {
                return this.RecurrenceDefinition;
            }

            set
            {
                this.RecurrenceDefinition = (DemoRecurrenceDefinition)value;
            }
        }

        public void PrepareNextTask()
        {
            int totalSeconds = this.randomizer.Next(this.MinDurationInSeconds, this.MaxDurationInSeconds);

            this.nextTask = new DemoTask(totalSeconds);

            this.nextSummary = this.nextTask.CreateTaskSummary(SummaryType.Text, true);

            this.nextPriority = this.nextPriority == TaskPriority.VeryHigh ? TaskPriority.Low : (TaskPriority)this.nextPriority++;
        }

        #endregion IScheduledTask Members
    }
}