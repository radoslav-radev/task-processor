using System;
using System.ComponentModel;
using System.Configuration;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// A serialization configuration for an entity type that reads from the App.config file.
    /// </summary>
    internal sealed class SerializationConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the type of the entity for which to apply the serialization configuration.
        /// </summary>
        /// <value>The type of the entity for which to apply the serialization configuration.</value>
        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        internal Type EntityType
        {
            get
            {
                return (Type)this["type"];
            }
        }

        /// <summary>
        /// Gets the type of the serializer which to instantiate in order to serialize or deserialize the entity.
        /// </summary>
        /// <value>The type of the serializer which to instantiate in order to serialize or deserialize the entity.</value>
        [ConfigurationProperty("serializer", IsRequired = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        internal Type SerializerType
        {
            get
            {
                return (Type)this["serializer"];
            }
        }
    }
}