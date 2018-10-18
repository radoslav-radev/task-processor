using System;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorRuntimeInfo"/> suitable for Redis.
    /// </summary>
    public sealed class RedisTaskProcessorRuntimeInfo : ITaskProcessorRuntimeInfo
    {
        private readonly Guid processorId;
        private readonly string machineName;

        private ITaskProcessorConfiguration configuration = new RedisTaskProcessorConfiguration();

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskProcessorRuntimeInfo"/> class.
        /// </summary>
        /// <param name="processorId">The unique ID of the task processor.</param>
        /// <param name="machineName">The name of the machine on which the task processor is hosted.</param>
        internal RedisTaskProcessorRuntimeInfo(Guid processorId, string machineName)
        {
            this.processorId = processorId;
            this.machineName = machineName;
        }

        #region ITaskProcessorRuntimeInfo Members

        /// <inheritdoc />
        public Guid TaskProcessorId
        {
            get { return this.processorId; }
        }

        /// <inheritdoc />
        public string MachineName
        {
            get { return this.machineName; }
        }

        /// <inheritdoc />
        public ITaskProcessorConfiguration Configuration
        {
            get { return this.configuration; }
        }

        #endregion ITaskProcessorRuntimeInfo Members

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.processorId.GetHashCode();
        }
    }
}