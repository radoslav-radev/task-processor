using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Facade;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskProcessorFacade : MockObject, ITaskProcessorFacade
    {
        #region ITaskProcessorFacade Members

        public void AddScheduledTask(IScheduledTask scheduledTask)
        {
            throw new NotImplementedException();
        }

        public void CancelTask(Guid taskId)
        {
            throw new NotImplementedException();
        }

        public void ClearTaskJobSettings(Type taskType)
        {
            this.RecordMethodCall(taskType);
        }

        public ISubmitTaskSession CreateSubmitTaskSession()
        {
            throw new NotImplementedException();
        }

        public void DeleteScheduledTask(Guid scheduledTaskId)
        {
            throw new NotImplementedException();
        }

        public Guid? GetMasterTaskProcessorId()
        {
            throw new NotImplementedException();
        }

        public ITask GetTask(Guid taskId)
        {
            throw new NotImplementedException();
        }

        public ITaskJobSettings GetTaskJobSettings(Type taskType)
        {
            this.RecordMethodCall(taskType);

            return this.GetPredefinedResultOrDefault<ITaskJobSettings>(taskType);
        }

        public IEnumerable<ITaskProcessorRuntimeInfo> GetTaskProcessorRuntimeInfo()
        {
            throw new NotImplementedException();
        }

        public ITaskProcessorRuntimeInfo GetTaskProcessorRuntimeInfo(Guid taskProcessorId)
        {
            throw new NotImplementedException();
        }

        public ITaskRuntimeInfo GetTaskRuntimeInfo(Guid taskId)
        {
            throw new NotImplementedException();
        }

        public ITaskSummary GetTaskSummary(Guid taskId)
        {
            throw new NotImplementedException();
        }

        public void MakeTaskProcessorMaster(Guid taskProcessorId)
        {
            throw new NotImplementedException();
        }

        public void RequestTaskProcessorToStop(Guid taskProcessorId)
        {
            throw new NotImplementedException();
        }

        public void SetTaskJobSettings(Type taskType, ITaskJobSettings settings)
        {
            this.RecordMethodCall(taskType, settings);

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
        }

        public Guid SubmitTask(ITask task)
        {
            throw new NotImplementedException();
        }

        public Guid SubmitTask(ITask task, TaskPriority priority)
        {
            throw new NotImplementedException();
        }

        public Guid SubmitTask(ITask task, ITaskSummary summary)
        {
            throw new NotImplementedException();
        }

        public Guid SubmitTask(ITask task, ITaskSummary summary, TaskPriority priority)
        {
            this.RecordMethodCall(task, summary, priority);

            return Guid.NewGuid();
        }

        public void UpdateScheduledTask(IScheduledTask scheduledTask)
        {
            throw new NotImplementedException();
        }

        public void UpdateTaskProcessorRuntimeInfo(ITaskProcessorRuntimeInfo taskProcessorInfo)
        {
            throw new NotImplementedException();
        }

        #endregion ITaskProcessorFacade Members
    }
}