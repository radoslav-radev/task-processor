using System;
using System.Configuration;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="ITaskWorkersConfiguration"/> that reads from App.config file.
    /// </summary>
    public sealed class SerializationConfigurationSection : ConfigurationSection, ITaskProcessorSerializationConfiguration
    {
        /// <summary>
        /// The section name in App.config file.
        /// </summary>
        public const string SectionName = "Radoslav.TaskProcessor.Serialization";

        [ConfigurationProperty("", IsDefaultCollection = true)]
        private SerializationConfigurationCollection EntityConfigurations
        {
            get
            {
                return (SerializationConfigurationCollection)this[string.Empty];
            }
        }

        #region ITaskProcessorSerializationConfiguration Members

        /// <inheritdoc />
        public Type GetSerializerType(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            SerializationConfigurationElement config = this.EntityConfigurations[entityType];

            if (config == null)
            {
                return null;
            }

            return config.SerializerType;
        }

        #endregion ITaskProcessorSerializationConfiguration Members
    }
}