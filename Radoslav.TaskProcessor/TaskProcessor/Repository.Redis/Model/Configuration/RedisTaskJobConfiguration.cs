using System;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskJobConfiguration"/> suitable for Redis.
    /// </summary>
    internal sealed class RedisTaskJobConfiguration : ITaskJobConfiguration
    {
        private readonly Type taskType;

        private int? maxWorkers;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskJobConfiguration"/> class.
        /// </summary>
        /// <param name="taskType">The type of task that defines the task job.</param>
        internal RedisTaskJobConfiguration(Type taskType)
        {
            this.taskType = taskType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskJobConfiguration"/> class by copying the properties of another instance.
        /// </summary>
        /// <param name="source">The instance whose properties to copy.</param>
        internal RedisTaskJobConfiguration(ITaskJobConfiguration source)
        {
            this.taskType = source.TaskType;
            this.maxWorkers = source.MaxWorkers;
        }

        #endregion Constructors

        #region ITaskJobConfiguration Members

        /// <inheritdoc />
        public Type TaskType
        {
            get { return this.taskType; }
        }

        #endregion ITaskJobConfiguration Members

        #region IMaxWorkersConfiguration

        /// <inheritdoc />
        public int? MaxWorkers
        {
            get
            {
                return this.maxWorkers;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Value must not be negative.");
                }

                this.maxWorkers = value;
            }
        }

        #endregion IMaxWorkersConfiguration

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.taskType.GetHashCode();
        }
    }
}