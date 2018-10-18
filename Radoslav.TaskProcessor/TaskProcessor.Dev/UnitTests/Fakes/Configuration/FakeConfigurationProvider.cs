using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeConfigurationProvider : MockObject, ITaskProcessorConfigurationProvider
    {
        #region ITaskProcessorConfigurationProvider Members

        public ITaskProcessorConfiguration GetTaskProcessorConfiguration()
        {
            this.RecordMethodCall();

            return this.GetPredefinedResultOrDefault<ITaskProcessorConfiguration>();
        }

        public ITaskWorkersConfiguration GetTaskWorkerConfiguration()
        {
            this.RecordMethodCall();

            return this.GetPredefinedResultOrDefault<ITaskWorkersConfiguration>();
        }

        public ITaskProcessorClientConfiguration GetClientConfiguration()
        {
            this.RecordMethodCall();

            return this.GetPredefinedResultOrDefault<ITaskProcessorClientConfiguration>();
        }

        public ITaskProcessorSerializationConfiguration GetSerializationConfiguration()
        {
            this.RecordMethodCall();

            return this.GetPredefinedResultOrDefault<ITaskProcessorSerializationConfiguration>();
        }

        public ITaskSchedulerConfiguration GetTaskSchedulerConfiguration()
        {
            this.RecordMethodCall();

            return this.GetPredefinedResultOrDefault<ITaskSchedulerConfiguration>();
        }

        #endregion ITaskProcessorConfigurationProvider Members
    }
}