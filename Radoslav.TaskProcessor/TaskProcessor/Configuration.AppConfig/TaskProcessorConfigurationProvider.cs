using Radoslav.Configuration;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorConfigurationProvider"/> that reads from App.config file.
    /// </summary>
    public sealed class TaskProcessorConfigurationProvider : ITaskProcessorConfigurationProvider
    {
        #region ITaskProcessorConfigurationProvider Members

        /// <inheritdoc />
        public ITaskProcessorConfiguration GetTaskProcessorConfiguration()
        {
            return ConfigurationHelpers.Load<TaskProcessorConfigurationSection>(TaskProcessorConfigurationSection.SectionName);
        }

        /// <inheritdoc />
        public ITaskWorkersConfiguration GetTaskWorkerConfiguration()
        {
            return ConfigurationHelpers.Load<TaskWorkerConfigurationSection>(TaskWorkerConfigurationSection.SectionName);
        }

        /// <inheritdoc />
        public ITaskProcessorClientConfiguration GetClientConfiguration()
        {
            return ConfigurationHelpers.Load<ClientConfigurationSection>(ClientConfigurationSection.SectionName);
        }

        /// <inheritdoc />
        public ITaskProcessorSerializationConfiguration GetSerializationConfiguration()
        {
            return ConfigurationHelpers.Load<SerializationConfigurationSection>(SerializationConfigurationSection.SectionName);
        }

        /// <inheritdoc />
        public ITaskSchedulerConfiguration GetTaskSchedulerConfiguration()
        {
            return ConfigurationHelpers.Load<TaskSchedulerConfigurationSection>(TaskSchedulerConfigurationSection.SectionName);
        }

        #endregion ITaskProcessorConfigurationProvider Members
    }
}