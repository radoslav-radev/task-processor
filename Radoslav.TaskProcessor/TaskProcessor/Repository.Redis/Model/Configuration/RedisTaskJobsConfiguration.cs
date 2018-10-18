using System;
using System.Collections;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskJobsConfiguration"/> suitable for Redis.
    /// </summary>
    internal sealed class RedisTaskJobsConfiguration : ITaskJobsConfiguration
    {
        private readonly Dictionary<Type, ITaskJobConfiguration> taskJobs = new Dictionary<Type, ITaskJobConfiguration>();

        private int? maxWorkers;

        #region ITaskJobsConfiguration Members

        /// <inheritdoc />
        public int? MaxWorkers
        {
            get
            {
                return this.maxWorkers;
            }

            set
            {
                if (value.HasValue && value.Value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Value must not be negative.");
                }

                this.maxWorkers = value;
            }
        }

        /// <inheritdoc />
        public ITaskJobConfiguration this[Type taskType]
        {
            get
            {
                ITaskJobConfiguration result;

                this.taskJobs.TryGetValue(taskType, out result);

                return result;
            }
        }

        /// <inheritdoc />
        public ITaskJobConfiguration Add(Type taskType)
        {
            if (this.taskJobs.ContainsKey(taskType))
            {
                throw new ArgumentException("Task job configuration for type '{0}' already exists.".FormatInvariant(taskType), "taskType");
            }

            ITaskJobConfiguration result = new RedisTaskJobConfiguration(taskType);

            this.taskJobs.Add(taskType, result);

            return result;
        }

        /// <inheritdoc />
        public void AddCopy(ITaskJobConfiguration source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (this.taskJobs.ContainsKey(source.TaskType))
            {
                throw new ArgumentException("Task job configuration for type '{0}' already exists.".FormatInvariant(source.TaskType), "source");
            }

            this.taskJobs.Add(source.TaskType, new RedisTaskJobConfiguration(source));
        }

        /// <inheritdoc />
        public void Remove(Type taskType)
        {
            this.taskJobs.Remove(taskType);
        }

        #endregion ITaskJobsConfiguration Members

        #region IEnumerable<ITaskJobConfiguration> Members

        /// <inheritdoc />
        public IEnumerator<ITaskJobConfiguration> GetEnumerator()
        {
            return this.taskJobs.Values.GetEnumerator();
        }

        #endregion IEnumerable<ITaskJobConfiguration> Members

        #region IEnumerable Members

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.taskJobs.Values.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}