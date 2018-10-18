using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorConfiguration"/> suitable for Redis.
    /// </summary>
    internal sealed class RedisTaskProcessorConfiguration : ITaskProcessorConfiguration
    {
        private readonly RedisTaskJobsConfiguration taskJobs = new RedisTaskJobsConfiguration();
        private readonly RedisPollingQueuesConfiguration pollingQueues = new RedisPollingQueuesConfiguration();
        private readonly RedisPollingJobsConfiguration pollingJobs = new RedisPollingJobsConfiguration();

        #region ITaskProcessorConfiguration Members

        /// <inheritdoc />
        public ITaskJobsConfiguration Tasks
        {
            get { return this.taskJobs; }
        }

        /// <inheritdoc />
        public ITaskProcessorPollingQueuesConfiguration PollingQueues
        {
            get { return this.pollingQueues; }
        }

        /// <inheritdoc />
        public IPollingJobsConfiguration PollingJobs
        {
            get { return this.pollingJobs; }
        }

        #endregion ITaskProcessorConfiguration Members
    }
}