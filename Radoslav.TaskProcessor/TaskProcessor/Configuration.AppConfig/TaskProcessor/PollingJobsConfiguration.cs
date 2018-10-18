using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="IPollingJobsConfiguration"/> that reads from App.config file.
    /// </summary>
    [ConfigurationCollection(typeof(IPollingJobConfiguration))]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = SuppressMessages.ConfigurationProperty)]
    internal sealed class PollingJobsConfiguration : ConfigurationElementCollection, IPollingJobsConfiguration
    {
        #region IPollingJobsConfiguration Members

        /// <inheritdoc />
        public IPollingJobConfiguration this[Type implementationType]
        {
            get
            {
                if (implementationType == null)
                {
                    throw new ArgumentNullException("implementationType");
                }

                if ((implementationType == typeof(IPollingJob)) || !typeof(IPollingJob).IsAssignableFrom(implementationType))
                {
                    throw new ArgumentException("Type '{0}' does not implement '{1}'.".FormatInvariant(implementationType, typeof(IPollingJob)), "implementationType");
                }

                return (IPollingJobConfiguration)this.BaseGet(implementationType);
            }
        }

        /// <inheritdoc />
        IPollingJobConfiguration IPollingJobsConfiguration.Add(Type implementationType)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        void IPollingJobsConfiguration.AddCopy(IPollingJobConfiguration source)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        void IPollingJobsConfiguration.Remove(Type implementationType)
        {
            throw new NotSupportedException();
        }

        #endregion IPollingJobsConfiguration Members

        #region IEnumerable<IPollingJobConfiguration> Members

        /// <inheritdoc />
        public new IEnumerator<IPollingJobConfiguration> GetEnumerator()
        {
            return this.OfType<IPollingJobConfiguration>().GetEnumerator();
        }

        #endregion IEnumerable<IPollingJobConfiguration> Members

        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new PollingJobConfiguration();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PollingJobConfiguration)element).ImplementationType;
        }
    }
}