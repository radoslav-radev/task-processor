using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.UnitTests
{
    [Serializable]
    public sealed class FakeSerializableScheduledTask : IScheduledTask
    {
        [NonSerialized]  // Needed because of binary serialization.
        private IScheduleDefinition schedule;

        public long Value { get; set; }

        [XmlIgnore]  // Needed because of XML serialization.
        [IgnoreDataMember] // Needed because of JSON serialization.
        public IScheduleDefinition Schedule
        {
            get { return this.schedule; }
            set { this.schedule = value; }
        }

        #region IScheduledTask Members

        public Guid Id { get; set; }

        public ITask NextTask
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TaskPriority NextTaskPriority
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ITaskSummary NextTaskSummary
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IScheduleDefinition IScheduledTask.Schedule
        {
            get { return this.schedule; }
            set { this.schedule = value; }
        }

        public void PrepareNextTask()
        {
            throw new NotImplementedException();
        }

        #endregion IScheduledTask Members
    }
}