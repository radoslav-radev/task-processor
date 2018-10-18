using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// A collection of task worker configurations that reads from the App.config file.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = SuppressMessages.ConfigurationProperty)]
    [ConfigurationCollection(typeof(TaskWorkerConfigurationElement))]
    internal sealed class TaskWorkerConfigurationCollection : ConfigurationElementCollection
    {
        #region ITaskWorkerTaskJobsConfiguration Members

        /// <summary>
        /// Gets a task worker configuration by a task type.
        /// </summary>
        /// <param name="taskType">The type of the task for which to retrieve task worker configuration.</param>
        /// <returns>Task worker configuration for the specified type, or null if not found.</returns>
        internal TaskWorkerConfigurationElement this[Type taskType]
        {
            get
            {
                return (TaskWorkerConfigurationElement)this.BaseGet(taskType);
            }
        }

        #endregion ITaskWorkerTaskJobsConfiguration Members

        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new TaskWorkerConfigurationElement();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TaskWorkerConfigurationElement)element).TaskType;
        }
    }
}