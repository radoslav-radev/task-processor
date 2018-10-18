using System;
using System.ComponentModel;
using System.Configuration;
using System.Xml;
using Radoslav.Configuration.Validators;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="IPollingJobConfiguration"/> that reads from App.config file.
    /// </summary>
    internal sealed class PollingJobConfiguration : PollingConfigurationElement, IPollingJobConfiguration
    {
        #region IPollingJobConfiguration Members

        /// <inheritdoc />
        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        [CompositeValidator(typeof(NotAbstractClassValidator))]
        public Type ImplementationType
        {
            get
            {
                return (Type)this["type"];
            }
        }

        #endregion IPollingJobConfiguration Members

        /// <inheritdoc />
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);

            AssignableFromTypeValidator validator = new AssignableFromTypeValidator(typeof(IPollingJob));

            try
            {
                validator.Validate(this.ImplementationType);
            }
            catch (ConfigurationErrorsException ex)
            {
                throw new ConfigurationErrorsException(ex.Message, reader);
            }
        }
    }
}