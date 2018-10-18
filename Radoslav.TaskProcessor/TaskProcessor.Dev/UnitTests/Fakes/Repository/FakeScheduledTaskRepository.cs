using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Repository;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeScheduledTaskRepository : MockObject, IScheduledTaskRepository
    {
        private readonly Dictionary<Guid, IScheduledTask> scheduledTasks = new Dictionary<Guid, IScheduledTask>();

        #region IScheduledTaskRepository Members

        public event EventHandler<ScheduledTaskEventArgs> Added;

        public event EventHandler<ScheduledTaskEventArgs> Updated;

        public event EventHandler<ScheduledTaskEventArgs> Deleted;

        public bool RaiseEvents { get; set; }

        public IEnumerable<IScheduledTask> GetAll()
        {
            return this.scheduledTasks.Values;
        }

        public IScheduledTask GetById(Guid scheduledTaskId)
        {
            IScheduledTask result;

            this.scheduledTasks.TryGetValue(scheduledTaskId, out result);

            return result;
        }

        public void Add(IScheduledTask scheduledTask)
        {
            this.RecordMethodCall(scheduledTask);

            if (scheduledTask == null)
            {
                throw new ArgumentNullException(nameof(scheduledTask));
            }

            this.scheduledTasks.Add(scheduledTask.Id, scheduledTask);

            if (this.RaiseEvents && (this.Added != null))
            {
                this.Added(this, new ScheduledTaskEventArgs(scheduledTask.Id));
            }
        }

        public void Update(IScheduledTask scheduledTask)
        {
            this.RecordMethodCall(scheduledTask);

            if (scheduledTask == null)
            {
                throw new ArgumentNullException(nameof(scheduledTask));
            }

            this.scheduledTasks[scheduledTask.Id] = (FakeSerializableScheduledTask)scheduledTask;

            if (this.RaiseEvents && (this.Updated != null))
            {
                this.Updated(this, new ScheduledTaskEventArgs(scheduledTask.Id));
            }
        }

        public void Delete(Guid scheduledTaskId)
        {
            this.RecordMethodCall(scheduledTaskId);

            this.scheduledTasks.Remove(scheduledTaskId);

            if (this.RaiseEvents && (this.Deleted != null))
            {
                this.Deleted(this, new ScheduledTaskEventArgs(scheduledTaskId));
            }
        }

        #endregion IScheduledTaskRepository Members
    }
}