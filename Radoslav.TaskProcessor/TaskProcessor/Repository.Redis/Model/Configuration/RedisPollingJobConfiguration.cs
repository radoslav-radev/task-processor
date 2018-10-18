using System;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="IPollingJobConfiguration"/> suitable for Redis.
    /// </summary>
    internal sealed class RedisPollingJobConfiguration : RedisPollingConfiguration, IPollingJobConfiguration
    {
        private readonly Type implementationType;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisPollingJobConfiguration"/> class.
        /// </summary>
        /// <param name="implementationType">The implementation type of the polling job configuration.</param>
        internal RedisPollingJobConfiguration(Type implementationType)
        {
            this.implementationType = implementationType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisPollingJobConfiguration"/> class by copying the properties of another instance.
        /// </summary>
        /// <param name="source">The instance whose properties to copy.</param>
        internal RedisPollingJobConfiguration(IPollingJobConfiguration source)
            : base(source)
        {
            this.implementationType = source.ImplementationType;
        }

        #endregion Constructors

        #region IPollingJobConfiguration Members

        /// <inheritdoc />
        public Type ImplementationType
        {
            get { return this.implementationType; }
        }

        #endregion IPollingJobConfiguration Members
    }
}