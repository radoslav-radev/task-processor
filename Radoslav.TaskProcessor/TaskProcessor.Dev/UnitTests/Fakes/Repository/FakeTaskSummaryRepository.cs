using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskSummaryRepository : MockObject, ITaskSummaryRepository
    {
        private readonly Dictionary<Guid, ITaskSummary> summaries = new Dictionary<Guid, ITaskSummary>();

        #region ITaskSummaryRepository Members

        public void Add(Guid taskId, ITaskSummary summary)
        {
            this.RecordMethodCall(taskId, summary);

            this.summaries.Add(taskId, summary);
        }

        public ITaskSummary GetById(Guid taskId)
        {
            this.RecordMethodCall(taskId);

            ITaskSummary result;

            this.summaries.TryGetValue(taskId, out result);

            return result;
        }

        #endregion ITaskSummaryRepository Members
    }
}