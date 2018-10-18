using System;
using System.Collections.Generic;
using System.Linq;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskRuntimeInfoRepository : MockObject, ITaskRuntimeInfoRepository
    {
        private readonly Dictionary<Guid, FakeTaskRuntimeInfo> taskInfos = new Dictionary<Guid, FakeTaskRuntimeInfo>();

        internal FakeTaskRuntimeInfo this[Guid taskId]
        {
            get
            {
                return this.taskInfos[taskId];
            }
        }

        #region ITaskRuntimeInfoRepository Members

        public IEnumerable<ITaskRuntimeInfo> GetPending(bool includePollingQueueTasks)
        {
            this.RecordMethodCall(includePollingQueueTasks);

            var result = this.taskInfos.Values.Where(t => t.Status == TaskStatus.Pending);

            if (!includePollingQueueTasks)
            {
                result = result.Where(t => string.IsNullOrEmpty(t.PollingQueue));
            }

            return result;
        }

        public IEnumerable<ITaskRuntimeInfo> ReservePollingQueueTasks(string pollingQueueKey, int maxResults)
        {
            this.RecordMethodCall(pollingQueueKey, maxResults);

            this.ExecutePredefinedMethod(pollingQueueKey, maxResults);

            return this.taskInfos.Values
                .Where(t => (t.PollingQueue == pollingQueueKey) && (t.Status == TaskStatus.Pending))
                .Take(maxResults)
                .ToArray();
        }

        public IEnumerable<ITaskRuntimeInfo> GetActive()
        {
            this.RecordMethodCall();

            return this.taskInfos.Values
                .Where(t => string.IsNullOrEmpty(t.PollingQueue) && (t.Status == TaskStatus.InProgress));
        }

        public IReadOnlyDictionary<TaskStatus, IEnumerable<ITaskRuntimeInfo>> GetPendingAndActive()
        {
            this.RecordMethodCall();

            Dictionary<TaskStatus, IEnumerable<ITaskRuntimeInfo>> result = this.taskInfos.Values
                .Where(t => string.IsNullOrEmpty(t.PollingQueue) && ((t.Status == TaskStatus.Pending) || (t.Status == TaskStatus.InProgress)))
                .GroupBy(t => t.Status)
                .ToDictionary(g => g.Key, g => (IEnumerable<ITaskRuntimeInfo>)g);

            if (!result.ContainsKey(TaskStatus.Pending))
            {
                result.Add(TaskStatus.Pending, Enumerable.Empty<ITaskRuntimeInfo>());
            }

            if (!result.ContainsKey(TaskStatus.InProgress))
            {
                result.Add(TaskStatus.InProgress, Enumerable.Empty<ITaskRuntimeInfo>());
            }

            return result;
        }

        public IEnumerable<ITaskRuntimeInfo> GetFailed()
        {
            this.RecordMethodCall();

            return this.taskInfos.Values.Where(t => t.Status == TaskStatus.Failed);
        }

        public IEnumerable<ITaskRuntimeInfo> GetArchive()
        {
            this.RecordMethodCall();

            return this.taskInfos.Values.Where(t => (t.Status == TaskStatus.Canceled) || (t.Status == TaskStatus.Failed) || (t.Status == TaskStatus.Success));
        }

        public ITaskRuntimeInfo GetById(Guid taskId)
        {
            this.RecordMethodCall(taskId);

            FakeTaskRuntimeInfo result;

            this.taskInfos.TryGetValue(taskId, out result);

            return result;
        }

        public Type GetTaskType(Guid taskId)
        {
            this.RecordMethodCall(taskId);

            return this.taskInfos[taskId].TaskType;
        }

        public ITaskRuntimeInfo Create(Guid taskId, Type taskType, DateTime submittedUtc, TaskPriority priority, string pollingQueue)
        {
            this.RecordMethodCall(taskId, taskType, submittedUtc, priority, pollingQueue);

            return new FakeTaskRuntimeInfo()
            {
                TaskId = taskId,
                TaskType = taskType,
                SubmittedUtc = submittedUtc,
                Priority = priority,
                PollingQueue = pollingQueue,
                Status = TaskStatus.Pending
            };
        }

        public void Add(ITaskRuntimeInfo taskInfo)
        {
            if (taskInfo == null)
            {
                throw new ArgumentNullException(nameof(taskInfo));
            }

            taskInfo.ValidateForAdd();

            this.RecordMethodCall(taskInfo);

            this.taskInfos.Add(taskInfo.TaskId, (FakeTaskRuntimeInfo)taskInfo);
        }

        public void Start(Guid taskId, Guid taskProcessorId, DateTime timestampUtc)
        {
            this.RecordMethodCall(taskId, taskProcessorId, timestampUtc);

            this.taskInfos[taskId].Status = TaskStatus.InProgress;
            this.taskInfos[taskId].TaskProcessorId = taskProcessorId;
            this.taskInfos[taskId].StartedUtc = timestampUtc;
        }

        public void RequestCancel(Guid taskId, DateTime timestampUtc)
        {
            this.RecordMethodCall(taskId, timestampUtc);

            this.taskInfos[taskId].Status = TaskStatus.Canceled;
            this.taskInfos[taskId].CanceledUtc = timestampUtc;
        }

        public void CompleteCancel(Guid taskId, DateTime timestampUtc)
        {
            this.RecordMethodCall(taskId, timestampUtc);

            this.taskInfos[taskId].CompletedUtc = timestampUtc;
        }

        public void Complete(Guid taskId, DateTime timestampUtc)
        {
            this.RecordMethodCall(taskId, timestampUtc);

            this.taskInfos[taskId].Status = TaskStatus.Success;
            this.taskInfos[taskId].CompletedUtc = timestampUtc;
        }

        public void Fail(Guid taskId, DateTime timestampUtc, Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            this.RecordMethodCall(taskId, timestampUtc, error);

            this.taskInfos[taskId].Status = TaskStatus.Failed;
            this.taskInfos[taskId].CompletedUtc = timestampUtc;
            this.taskInfos[taskId].Error = error.ToString();
        }

        public void Assign(Guid taskId, Guid? taskProcessorId)
        {
            this.RecordMethodCall(taskId, taskProcessorId);

            this.taskInfos[taskId].TaskProcessorId = taskProcessorId;
        }

        public void Progress(Guid taskId, double percentage)
        {
            this.RecordMethodCall(taskId, percentage);

            this.taskInfos[taskId].Percentage = percentage;
        }

        public bool CheckIsPendingOrActive(Guid taskId)
        {
            switch (this.taskInfos[taskId].Status)
            {
                case TaskStatus.Pending:
                case TaskStatus.InProgress:
                    return true;

                default:
                    return false;
            }
        }

        #endregion ITaskRuntimeInfoRepository Members
    }
}