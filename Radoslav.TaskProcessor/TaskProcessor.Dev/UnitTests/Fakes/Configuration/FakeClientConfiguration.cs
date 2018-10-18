using System;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeClientConfiguration : MockObject, ITaskProcessorClientConfiguration
    {
        #region ITaskProcessorClientConfiguration Members

        public string GetPollingQueueKey(Type taskType)
        {
            this.RecordMethodCall(taskType);

            return this.GetPredefinedResultOrDefault<string>(taskType);
        }

        #endregion ITaskProcessorClientConfiguration Members
    }
}