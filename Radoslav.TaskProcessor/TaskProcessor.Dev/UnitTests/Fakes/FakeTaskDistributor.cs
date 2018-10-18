using System;
using System.Collections.Generic;
using System.Linq;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.TaskDistributor;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskDistributor : MockObject, ITaskDistributor
    {
        #region ITaskDistributor Members

        public IEnumerable<ITaskProcessorRuntimeInfo> ChooseProcessorForTask(ITaskRuntimeInfo pendingTaskInfo)
        {
            this.RecordMethodCall(pendingTaskInfo);

            return this.GetPredefinedResultOrDefault<IEnumerable<ITaskProcessorRuntimeInfo>>(pendingTaskInfo) ?? Enumerable.Empty<ITaskProcessorRuntimeInfo>();
        }

        public IEnumerable<ITaskRuntimeInfo> ChooseNextTasksForProcessor(Guid taskProcessorId)
        {
            this.RecordMethodCall(taskProcessorId);

            return this.GetPredefinedResultOrDefault<IEnumerable<ITaskRuntimeInfo>>(taskProcessorId) ?? Enumerable.Empty<ITaskRuntimeInfo>();
        }

        #endregion ITaskDistributor Members
    }
}