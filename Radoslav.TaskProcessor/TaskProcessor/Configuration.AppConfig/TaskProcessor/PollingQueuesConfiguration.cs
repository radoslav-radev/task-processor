using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorPollingQueuesConfiguration"/> that reads from App.config file.
    /// </summary>
    [ConfigurationCollection(typeof(ITaskProcessorPollingQueueConfiguration))]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = SuppressMessages.ConfigurationProperty)]
    internal sealed class PollingQueuesConfiguration : ConfigurationElementCollection, ITaskProcessorPollingQueuesConfiguration
    {
        #region IPollingQueuesConfiguration Members

        /// <inheritdoc />
        public new ITaskProcessorPollingQueueConfiguration this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return (ITaskProcessorPollingQueueConfiguration)this.BaseGet(key);
            }
        }

        /// <inheritdoc />
        ITaskProcessorPollingQueueConfiguration ITaskProcessorPollingQueuesConfiguration.Add(string key)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        void ITaskProcessorPollingQueuesConfiguration.AddCopy(ITaskProcessorPollingQueueConfiguration source)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        void ITaskProcessorPollingQueuesConfiguration.Remove(string key)
        {
            throw new NotSupportedException();
        }

        #endregion IPollingQueuesConfiguration Members

        #region IEnumerable<IPollingQueueConfiguration> Members

        /// <inheritdoc />
        public new IEnumerator<ITaskProcessorPollingQueueConfiguration> GetEnumerator()
        {
            return this.OfType<ITaskProcessorPollingQueueConfiguration>().GetEnumerator();
        }

        #endregion IEnumerable<IPollingQueueConfiguration> Members

        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new PollingQueueConfiguration();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PollingQueueConfiguration)element).Key;
        }
    }
}