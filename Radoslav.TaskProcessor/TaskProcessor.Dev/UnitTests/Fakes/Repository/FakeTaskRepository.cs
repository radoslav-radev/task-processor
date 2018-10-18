using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskRepository : MockObject, ITaskRepository
    {
        private readonly Dictionary<Guid, FakeTask> tasks = new Dictionary<Guid, FakeTask>();

        #region ITaskRepository Members

        public ITask GetById(Guid taskId)
        {
            this.RecordMethodCall(taskId);

            FakeTask result;

            this.tasks.TryGetValue(taskId, out result);

            return result;
        }

        public void Add(Guid taskId, ITask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            this.RecordMethodCall(taskId, task);

            this.tasks.Add(taskId, (FakeTask)task);
        }

        public void Delete(Guid taskId)
        {
            this.RecordMethodCall(taskId);

            this.tasks.Remove(taskId);
        }

        #endregion ITaskRepository Members
    }
}