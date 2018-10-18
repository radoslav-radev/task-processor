using System;
using System.Configuration;
using System.Xml;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="IPollingConfiguration"/> that reads from App.config file.
    /// </summary>
    internal abstract class PollingConfigurationElement : ConfigurationElement, IPollingConfiguration
    {
        #region IPollingConfiguration Members

        /// <inheritdoc />
        [ConfigurationProperty("interval", IsRequired = true)]
        public TimeSpan PollInterval
        {
            get
            {
                return (TimeSpan)this["interval"];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <inheritdoc />
        [ConfigurationProperty("master")]
        public bool IsMaster
        {
            get
            {
                return (bool)this["master"];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <inheritdoc />
        [ConfigurationProperty("active", DefaultValue = true)]
        public bool IsActive
        {
            get
            {
                return (bool)this["active"];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <inheritdoc />
        [ConfigurationProperty("concurrent")]
        public bool IsConcurrent
        {
            get
            {
                return (bool)this["concurrent"];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion IPollingConfiguration Members

        /// <inheritdoc />
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);

            PositiveTimeSpanValidator validator = new PositiveTimeSpanValidator();

            try
            {
                validator.Validate(this.PollInterval);
            }
            catch (ConfigurationErrorsException ex)
            {
                throw new ConfigurationErrorsException(ex.Message, reader);
            }
        }
    }
}