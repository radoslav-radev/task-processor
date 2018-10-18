using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorConfiguration"/> that reads from App.config file.
    /// </summary>
    public sealed class TaskProcessorConfigurationSection : ConfigurationSection, ITaskProcessorConfiguration
    {
        /// <summary>
        /// The section name in App.config file.
        /// </summary>
        public const string SectionName = "Radoslav.TaskProcessor";

        #region ITaskProcessorConfiguration Members

        ITaskJobsConfiguration ITaskProcessorConfiguration.Tasks
        {
            get
            {
                return (ITaskJobsConfiguration)this["tasks"];
            }
        }

        ITaskProcessorPollingQueuesConfiguration ITaskProcessorConfiguration.PollingQueues
        {
            get
            {
                return (ITaskProcessorPollingQueuesConfiguration)this["pollingQueues"];
            }
        }

        IPollingJobsConfiguration ITaskProcessorConfiguration.PollingJobs
        {
            get
            {
                return (IPollingJobsConfiguration)this["pollingJobs"];
            }
        }

        #endregion ITaskProcessorConfiguration Members

        [ConfigurationProperty("tasks")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = SuppressMessages.ConfigurationProperty)]
        private TaskJobsConfigurationCollection Tasks
        {
            get
            {
                return (TaskJobsConfigurationCollection)this["tasks"];
            }
        }

        [ConfigurationProperty("pollingJobs")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = SuppressMessages.ConfigurationProperty)]
        private PollingJobsConfiguration PollingJobs
        {
            get
            {
                return (PollingJobsConfiguration)this["pollingJobs"];
            }
        }

        [ConfigurationProperty("pollingQueues")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = SuppressMessages.ConfigurationProperty)]
        private PollingQueuesConfiguration PollingQueues
        {
            get
            {
                return (PollingQueuesConfiguration)this["pollingQueues"];
            }
        }
    }
}