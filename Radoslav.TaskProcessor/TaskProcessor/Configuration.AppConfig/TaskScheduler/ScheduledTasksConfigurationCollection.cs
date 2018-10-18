using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of the <see cref="IScheduledTasksConfiguration"/> interface that reads from App.config file.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = SuppressMessages.ConfigurationProperty)]
    [ConfigurationCollection(typeof(ScheduledTaskConfigurationElement))]
    internal sealed class ScheduledTasksConfigurationCollection : ConfigurationElementCollection, IScheduledTasksConfiguration
    {
        #region IScheduledTasksConfiguration Members

        /// <inheritdoc />
        public IScheduledTaskConfiguration this[Type scheduledTaskType]
        {
            get
            {
                if (scheduledTaskType == null)
                {
                    throw new ArgumentNullException(nameof(scheduledTaskType));
                }

                if (!typeof(IScheduledTask).IsAssignableFrom(scheduledTaskType))
                {
                    throw new ArgumentException("Type '{0}' is not assignable from '{1}'.".FormatInvariant(scheduledTaskType, typeof(IScheduledTask), nameof(scheduledTaskType)));
                }

                return (IScheduledTaskConfiguration)this.BaseGet(scheduledTaskType);
            }
        }

        #endregion IScheduledTasksConfiguration Members

        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new ScheduledTaskConfigurationElement();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ScheduledTaskConfigurationElement)element).ScheduledTaskType;
        }
    }
}