using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// A collection of entity serialization configurations that reads from the App.config file.
    /// </summary>
    [ConfigurationCollection(typeof(SerializationConfigurationElement), AddItemName = "entity")]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = SuppressMessages.ConfigurationProperty)]
    internal sealed class SerializationConfigurationCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets the serialization configuration for an entity type.
        /// </summary>
        /// <param name="entityType">The type of the entity to serialize or deserialize.</param>
        /// <returns>The serialization configuration for the specified entity type, or null if not found.</returns>
        internal SerializationConfigurationElement this[Type entityType]
        {
            get
            {
                return (SerializationConfigurationElement)this.BaseGet(entityType);
            }
        }

        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new SerializationConfigurationElement();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SerializationConfigurationElement)element).EntityType;
        }
    }
}