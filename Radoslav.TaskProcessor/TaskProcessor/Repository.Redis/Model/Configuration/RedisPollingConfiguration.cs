using System;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="IPollingConfiguration"/> suitable for Redis.
    /// </summary>
    internal abstract class RedisPollingConfiguration : IPollingConfiguration
    {
        private TimeSpan interval;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisPollingConfiguration"/> class.
        /// </summary>
        internal RedisPollingConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisPollingConfiguration"/> class by copying the properties of another instance.
        /// </summary>
        /// <param name="source">The instance whose properties to copy.</param>
        internal RedisPollingConfiguration(IPollingConfiguration source)
        {
            this.PollInterval = source.PollInterval;
            this.IsActive = source.IsActive;
            this.IsMaster = source.IsMaster;
            this.IsConcurrent = source.IsConcurrent;
        }

        #endregion Constructors

        #region IPollingConfiguration Members

        /// <inheritdoc />
        public TimeSpan PollInterval
        {
            get
            {
                return this.interval;
            }

            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Value must be positive.");
                }

                this.interval = value;
            }
        }

        /// <inheritdoc />
        public bool IsMaster { get; set; }

        /// <inheritdoc />
        public bool IsActive { get; set; }

        /// <inheritdoc />
        public bool IsConcurrent { get; set; }

        #endregion IPollingConfiguration Members
    }
}