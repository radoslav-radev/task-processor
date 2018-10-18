using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakePollingQueuesConfiguration : ITaskProcessorPollingQueuesConfiguration
    {
        private readonly List<FakePollingQueueConfiguration> queues = new List<FakePollingQueueConfiguration>();

        #region IPollingQueuesConfiguration Members

        public ITaskProcessorPollingQueueConfiguration this[string key]
        {
            get
            {
                return this.queues.FirstOrDefault(q => q.Key == key);
            }
        }

        public ITaskProcessorPollingQueueConfiguration Add(string key)
        {
            FakePollingQueueConfiguration result = new FakePollingQueueConfiguration()
            {
                Key = key
            };

            this.queues.Add(result);

            return result;
        }

        public void AddCopy(ITaskProcessorPollingQueueConfiguration source)
        {
            this.queues.Add(new FakePollingQueueConfiguration(source));
        }

        public void Remove(string key)
        {
            this.queues.Remove(q => q.Key == key);
        }

        #endregion IPollingQueuesConfiguration Members

        #region IEnumerator<IPollingQueueConfiguration> Members

        public IEnumerator<ITaskProcessorPollingQueueConfiguration> GetEnumerator()
        {
            return this.queues.GetEnumerator();
        }

        #endregion IEnumerator<IPollingQueueConfiguration> Members

        #region IEnumerator Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.queues.GetEnumerator();
        }

        #endregion IEnumerator Members

        internal void Add(FakePollingQueueConfiguration queueConfig)
        {
            this.queues.Add(queueConfig);
        }
    }
}