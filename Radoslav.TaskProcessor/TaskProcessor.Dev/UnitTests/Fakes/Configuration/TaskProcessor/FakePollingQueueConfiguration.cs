using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakePollingQueueConfiguration : FakePollingConfiguration, ITaskProcessorPollingQueueConfiguration
    {
        internal FakePollingQueueConfiguration()
        {
        }

        internal FakePollingQueueConfiguration(ITaskProcessorPollingQueueConfiguration source)
            : base(source)
        {
            this.Key = source.Key;
            this.MaxWorkers = source.MaxWorkers;
        }

        #region IPollingQueueConfiguration Members

        public string Key { get; internal set; }

        public int MaxWorkers { get; set; }

        #endregion IPollingQueueConfiguration Members
    }
}