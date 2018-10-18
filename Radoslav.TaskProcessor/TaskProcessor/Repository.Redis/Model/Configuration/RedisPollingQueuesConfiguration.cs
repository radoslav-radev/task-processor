using System;
using System.Collections;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorPollingQueuesConfiguration"/> suitable for Redis.
    /// </summary>
    internal sealed class RedisPollingQueuesConfiguration : ITaskProcessorPollingQueuesConfiguration
    {
        private readonly Dictionary<string, ITaskProcessorPollingQueueConfiguration> pollingQueues = new Dictionary<string, ITaskProcessorPollingQueueConfiguration>();

        #region IPollingQueuesConfiguration Members

        /// <inheritdoc />
        public ITaskProcessorPollingQueueConfiguration this[string key]
        {
            get
            {
                ITaskProcessorPollingQueueConfiguration result;

                this.pollingQueues.TryGetValue(key, out result);

                return result;
            }
        }

        /// <inheritdoc />
        public ITaskProcessorPollingQueueConfiguration Add(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            RedisPollingQueueConfiguration result = new RedisPollingQueueConfiguration(key);

            this.pollingQueues.Add(key, result);

            return result;
        }

        /// <inheritdoc />
        public void AddCopy(ITaskProcessorPollingQueueConfiguration source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.pollingQueues.Add(source.Key, new RedisPollingQueueConfiguration(source));
        }

        /// <inheritdoc />
        public void Remove(string key)
        {
            this.pollingQueues.Remove(key);
        }

        #endregion IPollingQueuesConfiguration Members

        #region IEnumerator<IPollingQueueConfiguration> Members

        /// <inheritdoc />
        public IEnumerator<ITaskProcessorPollingQueueConfiguration> GetEnumerator()
        {
            return this.pollingQueues.Values.GetEnumerator();
        }

        #endregion IEnumerator<IPollingQueueConfiguration> Members

        #region IEnumerator Members

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.pollingQueues.Values.GetEnumerator();
        }

        #endregion IEnumerator Members
    }
}