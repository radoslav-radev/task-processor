using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Radoslav.Configuration.Validators;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="ITaskJobsConfiguration"/> that reads from App.config file.
    /// </summary>
    [ConfigurationCollection(typeof(TaskJobConfigurationElement))]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = SuppressMessages.ConfigurationProperty)]
    internal sealed class TaskJobsConfigurationCollection : ConfigurationElementCollection, ITaskJobsConfiguration
    {
        #region ITaskJobsConfiguration Members

        /// <inheritdoc />
        [ConfigurationProperty("max")]
        [NullableIntegerValidator(MinValue = 0)]
        public int? MaxWorkers
        {
            get
            {
                return (int?)this["max"];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <inheritdoc />
        public ITaskJobConfiguration this[Type taskType]
        {
            get
            {
                if (taskType == null)
                {
                    throw new ArgumentNullException("taskType");
                }

                return (ITaskJobConfiguration)this.BaseGet(taskType);
            }
        }

        /// <inheritdoc />
        ITaskJobConfiguration ITaskJobsConfiguration.Add(Type taskType)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        void ITaskJobsConfiguration.AddCopy(ITaskJobConfiguration taskJob)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        void ITaskJobsConfiguration.Remove(Type taskType)
        {
            throw new NotSupportedException();
        }

        #endregion ITaskJobsConfiguration Members

        #region IEnumerable<ITaskJobConfiguration> Members

        /// <inheritdoc />
        public new IEnumerator<ITaskJobConfiguration> GetEnumerator()
        {
            return this.OfType<ITaskJobConfiguration>().GetEnumerator();
        }

        #endregion IEnumerable<ITaskJobConfiguration> Members

        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new TaskJobConfigurationElement();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TaskJobConfigurationElement)element).TaskType;
        }
    }
}