using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskProcessorConfiguration : ITaskProcessorConfiguration
    {
        private readonly FakeTaskJobsConfiguration taskJobs = new FakeTaskJobsConfiguration();
        private readonly FakePollingQueuesConfiguration pollingQueues = new FakePollingQueuesConfiguration();
        private readonly FakePollingJobsConfiguration pollingJobs = new FakePollingJobsConfiguration();

        #region ITaskProcessorConfiguration Members

        public ITaskJobsConfiguration Tasks
        {
            get { return this.taskJobs; }
        }

        IPollingJobsConfiguration ITaskProcessorConfiguration.PollingJobs
        {
            get { return this.pollingJobs; }
        }

        ITaskProcessorPollingQueuesConfiguration ITaskProcessorConfiguration.PollingQueues
        {
            get { return this.pollingQueues; }
        }

        #endregion ITaskProcessorConfiguration Members

        internal FakePollingJobsConfiguration PollingJobs
        {
            get { return this.pollingJobs; }
        }

        internal FakePollingQueuesConfiguration PollingQueues
        {
            get { return this.pollingQueues; }
        }
    }
}