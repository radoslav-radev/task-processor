using System;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorPollingQueueConfiguration"/> suitable for Redis.
    /// </summary>
    internal sealed class RedisPollingQueueConfiguration : RedisPollingConfiguration, ITaskProcessorPollingQueueConfiguration
    {
        private readonly string key;
        private int maxWorkers;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisPollingQueueConfiguration"/> class.
        /// </summary>
        /// <param name="key">The unique key of the polling queue.</param>
        internal RedisPollingQueueConfiguration(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisPollingQueueConfiguration"/> class by copying the properties of another instance.
        /// </summary>
        /// <param name="source">The instance whose properties to copy.</param>
        internal RedisPollingQueueConfiguration(ITaskProcessorPollingQueueConfiguration source)
            : base(source)
        {
            this.key = source.Key;
            this.maxWorkers = source.MaxWorkers;
        }

        #endregion Constructors

        #region IPollingQueueConfiguration Members

        /// <inheritdoc />
        public string Key
        {
            get { return this.key; }
        }

        /// <inheritdoc />
        public int MaxWorkers
        {
            get
            {
                return this.maxWorkers;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value must not be negative.");
                }

                this.maxWorkers = value;
            }
        }

        #endregion IPollingQueueConfiguration Members
    }
}