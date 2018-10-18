using System;
using System.Configuration;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="ITaskWorkersConfiguration"/> that reads from App.config file.
    /// </summary>
    public sealed class TaskWorkerConfigurationSection : ConfigurationSection, ITaskWorkersConfiguration
    {
        /// <summary>
        /// The section name in App.config file.
        /// </summary>
        public const string SectionName = "Radoslav.TaskProcessor.TaskWorker";

        [ConfigurationProperty("", IsDefaultCollection = true)]
        private TaskWorkerConfigurationCollection TaskJobs
        {
            get
            {
                return (TaskWorkerConfigurationCollection)this[string.Empty];
            }
        }

        #region ITaskWorkerConfiguration Members

        /// <inheritdoc />
        public ITaskWorkerConfiguration this[Type taskType]
        {
            get
            {
                if (taskType == null)
                {
                    throw new ArgumentNullException(nameof(taskType));
                }

                if (!typeof(ITask).IsAssignableFrom(taskType))
                {
                    throw new ArgumentException("Type '{0}' does not inherit or implement '{1}'.".FormatInvariant(taskType, typeof(ITask)));
                }

                return this.TaskJobs[taskType];
            }
        }

        #endregion ITaskWorkerConfiguration Members
    }
}