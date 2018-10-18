using System;
using System.Configuration;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorPollingQueueConfiguration"/> that reads from App.config file.
    /// </summary>
    internal sealed class PollingQueueConfiguration : PollingConfigurationElement, ITaskProcessorPollingQueueConfiguration
    {
        #region IPollingQueueConfiguration Members

        /// <inheritdoc />
        [ConfigurationProperty("key", IsRequired = true, IsKey = true, DefaultValue = " ")]
        [StringValidator(MinLength = 1)]
        public string Key
        {
            get
            {
                return (string)this["key"];
            }
        }

        /// <inheritdoc />
        [ConfigurationProperty("max", IsRequired = true, DefaultValue = 10)]
        [IntegerValidator(MinValue = 0)]
        public int MaxWorkers
        {
            get
            {
                return (int)this["max"];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion IPollingQueueConfiguration Members
    }
}