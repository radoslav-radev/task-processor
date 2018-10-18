using System;
using System.ComponentModel;
using System.Configuration;
using Radoslav.Configuration.Validators;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of the <see cref="IScheduledTaskConfiguration"/> interface that reads from App.config file.
    /// </summary>
    internal sealed class ScheduledTaskConfigurationElement : ConfigurationElement, IScheduledTaskConfiguration
    {
        #region IScheduledTaskConfiguration Members

        /// <inheritdoc />
        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        [AssignableFromTypeValidator(typeof(IScheduledTask))]
        public Type ScheduledTaskType
        {
            get
            {
                return (Type)this["type"];
            }
        }

        /// <inheritdoc />
        [ConfigurationProperty("waitPrevious")]
        public bool WaitForPreviousSubmittedTaskToComplete
        {
            get
            {
                return (bool)this["waitPrevious"];
            }
        }

        #endregion IScheduledTaskConfiguration Members
    }
}