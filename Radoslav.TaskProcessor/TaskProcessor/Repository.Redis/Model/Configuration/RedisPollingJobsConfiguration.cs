using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="IPollingJobsConfiguration"/> suitable for Redis.
    /// </summary>
    internal sealed class RedisPollingJobsConfiguration : IPollingJobsConfiguration
    {
        private readonly Dictionary<Type, IPollingJobConfiguration> queues = new Dictionary<Type, IPollingJobConfiguration>();

        #region IPollingJobsConfiguration Members

        /// <inheritdoc />
        public IPollingJobConfiguration this[Type implementationType]
        {
            get
            {
                RepositoryExtensions.ValidatePollingJobImplementationType(implementationType);

                IPollingJobConfiguration result;

                this.queues.TryGetValue(implementationType, out result);

                return result;
            }
        }

        /// <inheritdoc />
        public IPollingJobConfiguration Add(Type implementationType)
        {
            RepositoryExtensions.ValidatePollingJobImplementationType(implementationType);

            RedisPollingJobConfiguration result = new RedisPollingJobConfiguration(implementationType);

            this.queues.Add(implementationType, result);

            return result;
        }

        /// <inheritdoc />
        public void AddCopy(IPollingJobConfiguration source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            RepositoryExtensions.ValidatePollingJobImplementationType(source.ImplementationType);

            this.queues.Add(source.ImplementationType, new RedisPollingJobConfiguration(source));
        }

        /// <inheritdoc />
        public void Remove(Type implementationType)
        {
            RepositoryExtensions.ValidatePollingJobImplementationType(implementationType);

            this.queues.Remove(implementationType);
        }

        #endregion IPollingJobsConfiguration Members

        #region IEnumerable<IPollingJobConfiguration> Members

        /// <inheritdoc />
        public IEnumerator<IPollingJobConfiguration> GetEnumerator()
        {
            return this.queues.Values.GetEnumerator();
        }

        #endregion IEnumerable<IPollingJobConfiguration> Members

        #region IEnumerable Members

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.queues.Values.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}